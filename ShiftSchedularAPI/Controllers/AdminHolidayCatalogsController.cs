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
    public class AdminHolidayCatalogsController : ControllerBase
    {
        private readonly DataContext _context;

        public AdminHolidayCatalogsController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.HolidayCatalogs
                .Include(hc => hc.HolidayType)
                .Include(hc => hc.HolidayBehaviour)
                .Include(hc => hc.HolidayCatalogLocalizations)
                    .ThenInclude(l => l.Localization)
                .OrderBy(hc => hc.HolidayName)
                .ToListAsync();

            return Ok(items.Select(MapToDTO));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.HolidayCatalogs
                .Include(hc => hc.HolidayType)
                .Include(hc => hc.HolidayBehaviour)
                .Include(hc => hc.HolidayCatalogLocalizations)
                    .ThenInclude(l => l.Localization)
                .FirstOrDefaultAsync(hc => hc.HolidayCatalogId == id);

            if (item == null) return NotFound();
            return Ok(MapToDTO(item));
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] AdminUpsertHolidayCatalogDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.HolidayName))
                return BadRequest("HolidayName is required.");

            HolidayCatalog item;

            if (dto.Id == 0)
            {
                item = new HolidayCatalog
                {
                    HolidayName = dto.HolidayName,
                    HolidayDescription = dto.HolidayDescription,
                    HolidayTypeId = dto.HolidayTypeId,
                    HolidayBehaviourId = dto.HolidayBehaviourId,
                    RecurrenceDay = dto.RecurrenceDay,
                    RecurrenceMonth = dto.RecurrenceMonth,
                    IsRecurring = dto.IsRecurring,
                    IsActive = dto.IsActive ? 1 : 0
                };
                _context.HolidayCatalogs.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                item = await _context.HolidayCatalogs.FindAsync(dto.Id);
                if (item == null) return NotFound();

                item.HolidayName = dto.HolidayName;
                item.HolidayDescription = dto.HolidayDescription;
                item.HolidayTypeId = dto.HolidayTypeId;
                item.HolidayBehaviourId = dto.HolidayBehaviourId;
                item.RecurrenceDay = dto.RecurrenceDay;
                item.RecurrenceMonth = dto.RecurrenceMonth;
                item.IsRecurring = dto.IsRecurring;
                item.IsActive = dto.IsActive ? 1 : 0;
                await _context.SaveChangesAsync();
            }

            await UpsertLocalizations(item.HolidayCatalogId, dto.Localizations);
            return Ok(new { id = item.HolidayCatalogId });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var localizations = await _context.HolidayCatalogLocalizations
                .Where(l => l.HolidayCatalogId == id).ToListAsync();
            _context.HolidayCatalogLocalizations.RemoveRange(localizations);

            var item = await _context.HolidayCatalogs.FindAsync(id);
            if (item != null) _context.HolidayCatalogs.Remove(item);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task UpsertLocalizations(int holidayCatalogId, List<AdminUpsertHolidayCatalogLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _context.HolidayCatalogLocalizations
                    .FirstOrDefaultAsync(l => l.HolidayCatalogId == holidayCatalogId && l.LocalizationId == loc.LocalizationId);

                if (existing == null)
                {
                    _context.HolidayCatalogLocalizations.Add(new HolidayCatalogLocalization
                    {
                        HolidayCatalogId = holidayCatalogId,
                        LocalizationId = loc.LocalizationId,
                        LocalizedName = loc.LocalizedName,
                        LocalizedDescription = loc.LocalizedDescription
                    });
                }
                else
                {
                    existing.LocalizedName = loc.LocalizedName;
                    existing.LocalizedDescription = loc.LocalizedDescription;
                }
            }
            await _context.SaveChangesAsync();
        }

        private static AdminHolidayCatalogItemDTO MapToDTO(HolidayCatalog hc) => new()
        {
            Id = hc.HolidayCatalogId,
            HolidayName = hc.HolidayName,
            HolidayDescription = hc.HolidayDescription,
            HolidayTypeId = hc.HolidayTypeId,
            HolidayTypeName = hc.HolidayType?.HolidayTypeName ?? string.Empty,
            HolidayBehaviourId = hc.HolidayBehaviourId,
            HolidayBehaviourName = hc.HolidayBehaviour?.HolidayBehaviourName ?? string.Empty,
            RecurrenceDay = hc.RecurrenceDay,
            RecurrenceMonth = hc.RecurrenceMonth,
            IsRecurring = hc.IsRecurring,
            IsActive = hc.IsActive == 1,
            Localizations = hc.HolidayCatalogLocalizations?.Select(l => new AdminHolidayCatalogLocalizationValueDTO
            {
                LocalizationId = l.LocalizationId,
                LocalizationCode = l.Localization?.LocalizationCode ?? string.Empty,
                LocalizedName = l.LocalizedName,
                LocalizedDescription = l.LocalizedDescription
            }).ToList() ?? new()
        };
    }
}
