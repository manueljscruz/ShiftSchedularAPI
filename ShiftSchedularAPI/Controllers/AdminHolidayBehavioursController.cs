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
    public class AdminHolidayBehavioursController : ControllerBase
    {
        private readonly DataContext _context;

        public AdminHolidayBehavioursController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.HolidayBehaviours
                .Include(hb => hb.HolidayBehaviourLocalizations)
                    .ThenInclude(l => l.Localization)
                .OrderBy(hb => hb.HolidayBehaviourName)
                .ToListAsync();

            return Ok(items.Select(MapToDTO));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.HolidayBehaviours
                .Include(hb => hb.HolidayBehaviourLocalizations)
                    .ThenInclude(l => l.Localization)
                .FirstOrDefaultAsync(hb => hb.HolidayBehaviourId == id);

            if (item == null) return NotFound();
            return Ok(MapToDTO(item));
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] AdminUpsertHolidayBehaviourDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.InternalName))
                return BadRequest("InternalName is required.");

            HolidayBehaviour item;

            if (dto.Id == 0)
            {
                item = new HolidayBehaviour
                {
                    HolidayBehaviourName = dto.InternalName,
                    AllowsOperatingTimes = dto.AllowsOperatingTimes
                };
                _context.HolidayBehaviours.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                item = await _context.HolidayBehaviours.FindAsync(dto.Id);
                if (item == null) return NotFound();

                item.HolidayBehaviourName = dto.InternalName;
                item.AllowsOperatingTimes = dto.AllowsOperatingTimes;
                await _context.SaveChangesAsync();
            }

            await UpsertLocalizations(item.HolidayBehaviourId, dto.Localizations);
            return Ok(new { id = item.HolidayBehaviourId });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var localizations = await _context.HolidayBehaviourLocalizations
                .Where(l => l.HolidayBehaviourId == id).ToListAsync();
            _context.HolidayBehaviourLocalizations.RemoveRange(localizations);

            var item = await _context.HolidayBehaviours.FindAsync(id);
            if (item != null) _context.HolidayBehaviours.Remove(item);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task UpsertLocalizations(int holidayBehaviourId, List<AdminUpsertLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _context.HolidayBehaviourLocalizations
                    .FirstOrDefaultAsync(l => l.HolidayBehaviourId == holidayBehaviourId && l.LocalizationId == loc.LocalizationId);

                if (existing == null)
                {
                    _context.HolidayBehaviourLocalizations.Add(new HolidayBehaviourLocalization
                    {
                        HolidayBehaviourId = holidayBehaviourId,
                        LocalizationId = loc.LocalizationId,
                        HolidayBehaviourDisplayValue = loc.DisplayValue
                    });
                }
                else
                {
                    existing.HolidayBehaviourDisplayValue = loc.DisplayValue;
                }
            }
            await _context.SaveChangesAsync();
        }

        private static AdminHolidayBehaviourItemDTO MapToDTO(HolidayBehaviour hb) => new()
        {
            Id = hb.HolidayBehaviourId,
            InternalName = hb.HolidayBehaviourName,
            AllowsOperatingTimes = hb.AllowsOperatingTimes,
            Localizations = hb.HolidayBehaviourLocalizations?.Select(l => new AdminLocalizationValueDTO
            {
                LocalizationId = l.LocalizationId,
                LocalizationCode = l.Localization?.LocalizationCode ?? string.Empty,
                DisplayValue = l.HolidayBehaviourDisplayValue
            }).ToList() ?? new()
        };
    }
}
