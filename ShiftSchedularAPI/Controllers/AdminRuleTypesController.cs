using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("general")]
    public class AdminRuleTypesController : ControllerBase
    {
        private readonly DataContext _context;

        public AdminRuleTypesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.RuleTypes
                .Include(rt => rt.RuleTypeLocalizations)
                    .ThenInclude(l => l.Localization)
                .Include(rt => rt.RuleTypeBusinessAspects)
                .OrderBy(rt => rt.OrderNo)
                    .ThenBy(rt => rt.RuleTypeName)
                .ToListAsync();

            return Ok(items.Select(MapToDTO));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.RuleTypes
                .Include(rt => rt.RuleTypeLocalizations)
                    .ThenInclude(l => l.Localization)
                .Include(rt => rt.RuleTypeBusinessAspects)
                .FirstOrDefaultAsync(rt => rt.RuleTypeId == id);

            if (item == null) return NotFound();
            return Ok(MapToDTO(item));
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] AdminUpsertRuleTypeDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.InternalName))
                return BadRequest("InternalName is required.");

            RuleType item;

            if (dto.Id == 0)
            {
                item = new RuleType
                {
                    RuleTypeName = dto.InternalName,
                    RuleTypeDescription = dto.Description,
                    MultipleSpecification = dto.MultipleSpecification,
                    IsSpecValuesBoolean = dto.IsSpecValuesBoolean,
                    OrderNo = dto.OrderNo
                };
                _context.RuleTypes.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                item = await _context.RuleTypes.FindAsync(dto.Id);
                if (item == null) return NotFound();

                item.RuleTypeName = dto.InternalName;
                item.RuleTypeDescription = dto.Description;
                item.MultipleSpecification = dto.MultipleSpecification;
                item.IsSpecValuesBoolean = dto.IsSpecValuesBoolean;
                item.OrderNo = dto.OrderNo;
                await _context.SaveChangesAsync();
            }

            await SyncBusinessAspects(item.RuleTypeId, dto.BusinessAspectIds);
            await UpsertLocalizations(item.RuleTypeId, dto.Localizations);
            return Ok(new { id = item.RuleTypeId });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var localizations = await _context.RuleTypeLocalizations
                .Where(l => l.RuleTypeId == id).ToListAsync();
            _context.RuleTypeLocalizations.RemoveRange(localizations);

            var businessAspects = await _context.RuleTypeBusinessAspects
                .Where(rba => rba.RuleTypeId == id).ToListAsync();
            _context.RuleTypeBusinessAspects.RemoveRange(businessAspects);

            var item = await _context.RuleTypes.FindAsync(id);
            if (item != null) _context.RuleTypes.Remove(item);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task SyncBusinessAspects(int ruleTypeId, List<int> businessAspectIds)
        {
            var existing = await _context.RuleTypeBusinessAspects
                .Where(rba => rba.RuleTypeId == ruleTypeId).ToListAsync();

            var toRemove = existing.Where(e => !businessAspectIds.Contains(e.BusinessAspectId)).ToList();
            _context.RuleTypeBusinessAspects.RemoveRange(toRemove);

            foreach (var baId in businessAspectIds)
            {
                if (!existing.Any(e => e.BusinessAspectId == baId))
                {
                    _context.RuleTypeBusinessAspects.Add(new RuleTypeBusinessAspect
                    {
                        RuleTypeId = ruleTypeId,
                        BusinessAspectId = baId
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        private async Task UpsertLocalizations(int ruleTypeId, List<AdminUpsertRuleTypeLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _context.RuleTypeLocalizations
                    .FirstOrDefaultAsync(l => l.RuleTypeId == ruleTypeId && l.LocalizationId == loc.LocalizationId);

                if (existing == null)
                {
                    _context.RuleTypeLocalizations.Add(new RuleTypeLocalization
                    {
                        RuleTypeId = ruleTypeId,
                        LocalizationId = loc.LocalizationId,
                        RuleTypeDisplayValue = loc.DisplayValue,
                        RuleTypeDescriptionDisplayValue = loc.DescriptionValue
                    });
                }
                else
                {
                    existing.RuleTypeDisplayValue = loc.DisplayValue;
                    existing.RuleTypeDescriptionDisplayValue = loc.DescriptionValue;
                }
            }
            await _context.SaveChangesAsync();
        }

        private static AdminRuleTypeItemDTO MapToDTO(RuleType rt) => new()
        {
            Id = rt.RuleTypeId,
            InternalName = rt.RuleTypeName,
            Description = rt.RuleTypeDescription,
            MultipleSpecification = rt.MultipleSpecification,
            IsSpecValuesBoolean = rt.IsSpecValuesBoolean,
            OrderNo = rt.OrderNo,
            BusinessAspectIds = rt.RuleTypeBusinessAspects?.Select(rba => rba.BusinessAspectId).ToList() ?? new(),
            Localizations = rt.RuleTypeLocalizations?.Select(l => new AdminRuleTypeLocalizationValueDTO
            {
                LocalizationId = l.LocalizationId,
                LocalizationCode = l.Localization?.LocalizationCode ?? string.Empty,
                DisplayValue = l.RuleTypeDisplayValue,
                DescriptionValue = l.RuleTypeDescriptionDisplayValue
            }).ToList() ?? new()
        };
    }
}
