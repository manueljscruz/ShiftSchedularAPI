using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.Admin.Handlers
{
    public class BusinessAspectTypeHandler : IAdminTypeHandler
    {
        private readonly DataContext _context;

        public BusinessAspectTypeHandler(DataContext context)
        {
            _context = context;
        }

        public bool HasColors => false;

        public async Task<List<AdminTypeItemDTO>> GetAllAsync()
        {
            var items = await _context.BusinessAspects
                .Include(ba => ba.BusinessAspectLocalizations)
                    .ThenInclude(bal => bal.Localization)
                .OrderBy(ba => ba.BusinessAspectName)
                .ToListAsync();

            return items.Select(MapToDTO).ToList();
        }

        public async Task<AdminTypeItemDTO?> GetByIdAsync(int id)
        {
            var item = await _context.BusinessAspects
                .Include(ba => ba.BusinessAspectLocalizations)
                    .ThenInclude(bal => bal.Localization)
                .FirstOrDefaultAsync(ba => ba.BusinessAspectId == id);

            return item == null ? null : MapToDTO(item);
        }

        public async Task<int> UpsertAsync(AdminUpsertTypeDTO dto)
        {
            BusinessAspect item;

            if (dto.Id == 0)
            {
                item = new BusinessAspect { BusinessAspectName = dto.InternalName };
                _context.BusinessAspects.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                item = await _context.BusinessAspects.FindAsync(dto.Id);
                if (item == null) return 0;

                item.BusinessAspectName = dto.InternalName;
                await _context.SaveChangesAsync();
            }

            await UpsertLocalizations(item.BusinessAspectId, dto.Localizations);
            return item.BusinessAspectId;
        }

        public async Task DeleteAsync(int id)
        {
            var localizations = await _context.BusinessAspectLocalizations
                .Where(bal => bal.BusinessAspectId == id).ToListAsync();
            _context.BusinessAspectLocalizations.RemoveRange(localizations);

            var item = await _context.BusinessAspects.FindAsync(id);
            if (item != null) _context.BusinessAspects.Remove(item);

            await _context.SaveChangesAsync();
        }

        private async Task UpsertLocalizations(int businessAspectId, List<AdminUpsertLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _context.BusinessAspectLocalizations
                    .FirstOrDefaultAsync(bal => bal.BusinessAspectId == businessAspectId && bal.LocalizationId == loc.LocalizationId);

                if (existing == null)
                {
                    _context.BusinessAspectLocalizations.Add(new BusinessAspectLocalization
                    {
                        BusinessAspectId = businessAspectId,
                        LocalizationId = loc.LocalizationId,
                        BusinessAspectDisplayValue = loc.DisplayValue
                    });
                }
                else
                {
                    existing.BusinessAspectDisplayValue = loc.DisplayValue;
                }
            }
            await _context.SaveChangesAsync();
        }

        private static AdminTypeItemDTO MapToDTO(BusinessAspect ba) => new()
        {
            Id = ba.BusinessAspectId,
            InternalName = ba.BusinessAspectName,
            Localizations = ba.BusinessAspectLocalizations?.Select(bal => new AdminLocalizationValueDTO
            {
                LocalizationId = bal.LocalizationId,
                LocalizationCode = bal.Localization?.LocalizationCode ?? string.Empty,
                DisplayValue = bal.BusinessAspectDisplayValue
            }).ToList() ?? new()
        };
    }
}
