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
    public class AdminNotificationTypesController : ControllerBase
    {
        private readonly DataContext _context;

        public AdminNotificationTypesController(DataContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var items = await _context.NotificationTypes
                .Include(nt => nt.NotificationTypeLocalizations)
                    .ThenInclude(l => l.Localization)
                .OrderBy(nt => nt.NotificationTypeCode)
                .ToListAsync();

            return Ok(items.Select(MapToDTO));
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id)
        {
            var item = await _context.NotificationTypes
                .Include(nt => nt.NotificationTypeLocalizations)
                    .ThenInclude(l => l.Localization)
                .FirstOrDefaultAsync(nt => nt.NotificationTypeId == id);

            if (item == null) return NotFound();
            return Ok(MapToDTO(item));
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] AdminUpsertNotificationTypeDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.NotificationTypeCode))
                return BadRequest("NotificationTypeCode is required.");

            NotificationType item;

            if (dto.Id == 0)
            {
                item = new NotificationType { NotificationTypeCode = dto.NotificationTypeCode };
                _context.NotificationTypes.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                item = await _context.NotificationTypes.FindAsync(dto.Id);
                if (item == null) return NotFound();

                item.NotificationTypeCode = dto.NotificationTypeCode;
                await _context.SaveChangesAsync();
            }

            await UpsertLocalizations(item.NotificationTypeId, dto.Localizations);
            return Ok(new { id = item.NotificationTypeId });
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var localizations = await _context.NotificationTypeLocalizations
                .Where(l => l.NotificationTypeId == id).ToListAsync();
            _context.NotificationTypeLocalizations.RemoveRange(localizations);

            var item = await _context.NotificationTypes.FindAsync(id);
            if (item != null) _context.NotificationTypes.Remove(item);

            await _context.SaveChangesAsync();
            return NoContent();
        }

        private async Task UpsertLocalizations(int notificationTypeId, List<AdminUpsertNotificationTypeLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _context.NotificationTypeLocalizations
                    .FirstOrDefaultAsync(l => l.NotificationTypeId == notificationTypeId && l.LocalizationId == loc.LocalizationId);

                if (existing == null)
                {
                    _context.NotificationTypeLocalizations.Add(new NotificationTypeLocalization
                    {
                        NotificationTypeId = notificationTypeId,
                        LocalizationId = loc.LocalizationId,
                        NotificationTypeDisplayValue = loc.DisplayValue,
                        MessageTemplate = loc.MessageTemplate
                    });
                }
                else
                {
                    existing.NotificationTypeDisplayValue = loc.DisplayValue;
                    existing.MessageTemplate = loc.MessageTemplate;
                }
            }
            await _context.SaveChangesAsync();
        }

        private static AdminNotificationTypeItemDTO MapToDTO(NotificationType nt) => new()
        {
            Id = nt.NotificationTypeId,
            NotificationTypeCode = nt.NotificationTypeCode,
            Localizations = nt.NotificationTypeLocalizations?.Select(l => new AdminNotificationTypeLocalizationValueDTO
            {
                LocalizationId = l.LocalizationId,
                LocalizationCode = l.Localization?.LocalizationCode ?? string.Empty,
                DisplayValue = l.NotificationTypeDisplayValue,
                MessageTemplate = l.MessageTemplate
            }).ToList() ?? new()
        };
    }
}
