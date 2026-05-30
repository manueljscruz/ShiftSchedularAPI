using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.Admin.Handlers
{
    public class LocalizationTypeHandler : IAdminTypeHandler
    {
        private readonly DataContext _context;

        public LocalizationTypeHandler(DataContext context)
        {
            _context = context;
        }

        public bool HasColors => false;

        public async Task<List<AdminTypeItemDTO>> GetAllAsync()
        {
            var items = await _context.Localizations
                .OrderBy(l => l.LocalizationCode)
                .ToListAsync();

            return items.Select(MapToDTO).ToList();
        }

        public async Task<AdminTypeItemDTO?> GetByIdAsync(int id)
        {
            var item = await _context.Localizations.FindAsync(id);
            return item == null ? null : MapToDTO(item);
        }

        public async Task<int> UpsertAsync(AdminUpsertTypeDTO dto)
        {
            if (dto.Id == 0)
            {
                var item = new Localization { LocalizationCode = dto.InternalName };
                _context.Localizations.Add(item);
                await _context.SaveChangesAsync();
                return item.LocalizationId;
            }
            else
            {
                var item = await _context.Localizations.FindAsync(dto.Id);
                if (item == null) return 0;

                item.LocalizationCode = dto.InternalName;
                await _context.SaveChangesAsync();
                return item.LocalizationId;
            }
        }

        public async Task DeleteAsync(int id)
        {
            var item = await _context.Localizations.FindAsync(id);
            if (item != null)
            {
                _context.Localizations.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        private static AdminTypeItemDTO MapToDTO(Localization l) => new()
        {
            Id = l.LocalizationId,
            InternalName = l.LocalizationCode,
            Localizations = new()
        };
    }
}
