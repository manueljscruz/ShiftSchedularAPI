using Microsoft.EntityFrameworkCore;
using ShiftSchedularBLL.Admin;
using ShiftSchedularDAL.Data;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.Admin.Handlers
{
    public class AbsenceTypeTypeHandler : IAdminTypeHandler
    {
        private readonly DataContext _context;

        public AbsenceTypeTypeHandler(DataContext context)
        {
            _context = context;
        }

        public bool HasColors => false;

        public async Task<List<AdminTypeItemDTO>> GetAllAsync()
        {
            var items = await _context.AbsenceTypes
                .Include(at => at.AbsenceTypeLocalizations)
                    .ThenInclude(atl => atl.Localization)
                .OrderBy(at => at.AbsenceTypeName)
                .ToListAsync();

            return items.Select(MapToDTO).ToList();
        }

        public async Task<AdminTypeItemDTO?> GetByIdAsync(int id)
        {
            var item = await _context.AbsenceTypes
                .Include(at => at.AbsenceTypeLocalizations)
                    .ThenInclude(atl => atl.Localization)
                .FirstOrDefaultAsync(at => at.AbsenceTypeId == id);

            return item == null ? null : MapToDTO(item);
        }

        public async Task<int> UpsertAsync(AdminUpsertTypeDTO dto)
        {
            AbsenceType item;

            if (dto.Id == 0)
            {
                item = new AbsenceType { AbsenceTypeName = dto.InternalName };
                _context.AbsenceTypes.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                item = await _context.AbsenceTypes.FindAsync(dto.Id);
                if (item == null) return 0;

                item.AbsenceTypeName = dto.InternalName;
                await _context.SaveChangesAsync();
            }

            await UpsertLocalizations(item.AbsenceTypeId, dto.Localizations);
            return item.AbsenceTypeId;
        }

        public async Task DeleteAsync(int id)
        {
            var localizations = await _context.AbsenceTypeLocalizations
                .Where(atl => atl.AbsenceTypeId == id).ToListAsync();
            _context.AbsenceTypeLocalizations.RemoveRange(localizations);

            var item = await _context.AbsenceTypes.FindAsync(id);
            if (item != null) _context.AbsenceTypes.Remove(item);

            await _context.SaveChangesAsync();
        }

        private async Task UpsertLocalizations(int absenceTypeId, List<AdminUpsertLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _context.AbsenceTypeLocalizations
                    .FirstOrDefaultAsync(atl => atl.AbsenceTypeId == absenceTypeId && atl.LocalizationId == loc.LocalizationId);

                if (existing == null)
                {
                    _context.AbsenceTypeLocalizations.Add(new AbsenceTypeLocalization
                    {
                        AbsenceTypeId = absenceTypeId,
                        LocalizationId = loc.LocalizationId,
                        AbsenceTypeDisplayValue = loc.DisplayValue
                    });
                }
                else
                {
                    existing.AbsenceTypeDisplayValue = loc.DisplayValue;
                }
            }
            await _context.SaveChangesAsync();
        }

        private static AdminTypeItemDTO MapToDTO(AbsenceType at) => new()
        {
            Id = at.AbsenceTypeId,
            InternalName = at.AbsenceTypeName,
            Localizations = at.AbsenceTypeLocalizations?.Select(atl => new AdminLocalizationValueDTO
            {
                LocalizationId = atl.LocalizationId,
                LocalizationCode = atl.Localization?.LocalizationCode ?? string.Empty,
                DisplayValue = atl.AbsenceTypeDisplayValue
            }).ToList() ?? new()
        };
    }
}
