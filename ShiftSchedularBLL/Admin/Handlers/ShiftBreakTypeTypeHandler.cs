using Microsoft.EntityFrameworkCore;
using ShiftSchedularBLL.Admin;
using ShiftSchedularDAL.Data;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.Admin.Handlers
{
    public class ShiftBreakTypeTypeHandler : IAdminTypeHandler
    {
        private readonly DataContext _context;

        public ShiftBreakTypeTypeHandler(DataContext context)
        {
            _context = context;
        }

        public bool HasColors => false;

        public async Task<List<AdminTypeItemDTO>> GetAllAsync()
        {
            var items = await _context.ShiftBreakTypes
                .Include(sbt => sbt.ShiftBreakTypeLocalizations)
                    .ThenInclude(sbtl => sbtl.Localization)
                .OrderBy(sbt => sbt.ShiftBreakTypeValue)
                .ToListAsync();

            return items.Select(MapToDTO).ToList();
        }

        public async Task<AdminTypeItemDTO?> GetByIdAsync(int id)
        {
            var item = await _context.ShiftBreakTypes
                .Include(sbt => sbt.ShiftBreakTypeLocalizations)
                    .ThenInclude(sbtl => sbtl.Localization)
                .FirstOrDefaultAsync(sbt => sbt.ShiftBreakTypeId == id);

            return item == null ? null : MapToDTO(item);
        }

        public async Task<int> UpsertAsync(AdminUpsertTypeDTO dto)
        {
            ShiftBreakType item;

            if (dto.Id == 0)
            {
                item = new ShiftBreakType { ShiftBreakTypeValue = dto.InternalName };
                _context.ShiftBreakTypes.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                item = await _context.ShiftBreakTypes.FindAsync(dto.Id);
                if (item == null) return 0;

                item.ShiftBreakTypeValue = dto.InternalName;
                await _context.SaveChangesAsync();
            }

            await UpsertLocalizations(item.ShiftBreakTypeId, dto.Localizations);
            return item.ShiftBreakTypeId;
        }

        public async Task DeleteAsync(int id)
        {
            var localizations = await _context.ShiftBreakTypeLocalizations
                .Where(sbtl => sbtl.ShiftBreakTypeId == id).ToListAsync();
            _context.ShiftBreakTypeLocalizations.RemoveRange(localizations);

            var item = await _context.ShiftBreakTypes.FindAsync(id);
            if (item != null) _context.ShiftBreakTypes.Remove(item);

            await _context.SaveChangesAsync();
        }

        private async Task UpsertLocalizations(int shiftBreakTypeId, List<AdminUpsertLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _context.ShiftBreakTypeLocalizations
                    .FirstOrDefaultAsync(sbtl => sbtl.ShiftBreakTypeId == shiftBreakTypeId && sbtl.LocalizationId == loc.LocalizationId);

                if (existing == null)
                {
                    _context.ShiftBreakTypeLocalizations.Add(new ShiftBreakTypeLocalization
                    {
                        ShiftBreakTypeId = shiftBreakTypeId,
                        LocalizationId = loc.LocalizationId,
                        ShiftBreakTypeDisplayValue = loc.DisplayValue
                    });
                }
                else
                {
                    existing.ShiftBreakTypeDisplayValue = loc.DisplayValue;
                }
            }
            await _context.SaveChangesAsync();
        }

        private static AdminTypeItemDTO MapToDTO(ShiftBreakType sbt) => new()
        {
            Id = sbt.ShiftBreakTypeId,
            InternalName = sbt.ShiftBreakTypeValue,
            Localizations = sbt.ShiftBreakTypeLocalizations?.Select(sbtl => new AdminLocalizationValueDTO
            {
                LocalizationId = sbtl.LocalizationId,
                LocalizationCode = sbtl.Localization?.LocalizationCode ?? string.Empty,
                DisplayValue = sbtl.ShiftBreakTypeDisplayValue
            }).ToList() ?? new()
        };
    }
}
