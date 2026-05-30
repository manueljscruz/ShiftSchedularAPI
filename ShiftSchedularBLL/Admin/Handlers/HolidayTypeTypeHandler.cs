using Microsoft.EntityFrameworkCore;
using ShiftSchedularBLL.Admin;
using ShiftSchedularDAL.Data;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.Admin.Handlers
{
    public class HolidayTypeTypeHandler : IAdminTypeHandler
    {
        private readonly DataContext _context;

        public HolidayTypeTypeHandler(DataContext context)
        {
            _context = context;
        }

        public bool HasColors => false;

        public async Task<List<AdminTypeItemDTO>> GetAllAsync()
        {
            var items = await _context.HolidayTypes
                .Include(ht => ht.HolidayTypeLocalizations)
                    .ThenInclude(htl => htl.Localization)
                .OrderBy(ht => ht.HolidayTypeName)
                .ToListAsync();

            return items.Select(MapToDTO).ToList();
        }

        public async Task<AdminTypeItemDTO?> GetByIdAsync(int id)
        {
            var item = await _context.HolidayTypes
                .Include(ht => ht.HolidayTypeLocalizations)
                    .ThenInclude(htl => htl.Localization)
                .FirstOrDefaultAsync(ht => ht.HolidayTypeId == id);

            return item == null ? null : MapToDTO(item);
        }

        public async Task<int> UpsertAsync(AdminUpsertTypeDTO dto)
        {
            HolidayType item;

            if (dto.Id == 0)
            {
                item = new HolidayType { HolidayTypeName = dto.InternalName };
                _context.HolidayTypes.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                item = await _context.HolidayTypes.FindAsync(dto.Id);
                if (item == null) return 0;

                item.HolidayTypeName = dto.InternalName;
                await _context.SaveChangesAsync();
            }

            await UpsertLocalizations(item.HolidayTypeId, dto.Localizations);
            return item.HolidayTypeId;
        }

        public async Task DeleteAsync(int id)
        {
            var localizations = await _context.HolidayTypeLocalizations
                .Where(htl => htl.HolidayTypeId == id).ToListAsync();
            _context.HolidayTypeLocalizations.RemoveRange(localizations);

            var item = await _context.HolidayTypes.FindAsync(id);
            if (item != null) _context.HolidayTypes.Remove(item);

            await _context.SaveChangesAsync();
        }

        private async Task UpsertLocalizations(int holidayTypeId, List<AdminUpsertLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _context.HolidayTypeLocalizations
                    .FirstOrDefaultAsync(htl => htl.HolidayTypeId == holidayTypeId && htl.LocalizationId == loc.LocalizationId);

                if (existing == null)
                {
                    _context.HolidayTypeLocalizations.Add(new HolidayTypeLocalization
                    {
                        HolidayTypeId = holidayTypeId,
                        LocalizationId = loc.LocalizationId,
                        HolidayTypeDisplayValue = loc.DisplayValue
                    });
                }
                else
                {
                    existing.HolidayTypeDisplayValue = loc.DisplayValue;
                }
            }
            await _context.SaveChangesAsync();
        }

        private static AdminTypeItemDTO MapToDTO(HolidayType ht) => new()
        {
            Id = ht.HolidayTypeId,
            InternalName = ht.HolidayTypeName,
            Localizations = ht.HolidayTypeLocalizations?.Select(htl => new AdminLocalizationValueDTO
            {
                LocalizationId = htl.LocalizationId,
                LocalizationCode = htl.Localization?.LocalizationCode ?? string.Empty,
                DisplayValue = htl.HolidayTypeDisplayValue
            }).ToList() ?? new()
        };
    }
}
