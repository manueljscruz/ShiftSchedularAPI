using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.Admin.Handlers
{
    public class EntityPermissionRoleTypeHandler : IAdminTypeHandler
    {
        private readonly DataContext _context;

        public EntityPermissionRoleTypeHandler(DataContext context)
        {
            _context = context;
        }

        public bool HasColors => false;

        public async Task<List<AdminTypeItemDTO>> GetAllAsync()
        {
            var items = await _context.EntityPermissionRoles
                .Include(epr => epr.EntityPermissionRoleLocalizations)
                    .ThenInclude(eprl => eprl.Localization)
                .OrderBy(epr => epr.EntityPermissionRoleName)
                .ToListAsync();

            return items.Select(MapToDTO).ToList();
        }

        public async Task<AdminTypeItemDTO?> GetByIdAsync(int id)
        {
            var item = await _context.EntityPermissionRoles
                .Include(epr => epr.EntityPermissionRoleLocalizations)
                    .ThenInclude(eprl => eprl.Localization)
                .FirstOrDefaultAsync(epr => epr.EntityPermissionRoleId == id);

            return item == null ? null : MapToDTO(item);
        }

        public async Task<int> UpsertAsync(AdminUpsertTypeDTO dto)
        {
            EntityPermissionRole item;

            if (dto.Id == 0)
            {
                item = new EntityPermissionRole { EntityPermissionRoleName = dto.InternalName };
                _context.EntityPermissionRoles.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                item = await _context.EntityPermissionRoles.FindAsync(dto.Id);
                if (item == null) return 0;

                item.EntityPermissionRoleName = dto.InternalName;
                await _context.SaveChangesAsync();
            }

            await UpsertLocalizations(item.EntityPermissionRoleId, dto.Localizations);
            return item.EntityPermissionRoleId;
        }

        public async Task DeleteAsync(int id)
        {
            var localizations = await _context.EntityPermissionRoleLocalizations
                .Where(eprl => eprl.EntityPermissionRoleId == id).ToListAsync();
            _context.EntityPermissionRoleLocalizations.RemoveRange(localizations);

            var item = await _context.EntityPermissionRoles.FindAsync(id);
            if (item != null) _context.EntityPermissionRoles.Remove(item);

            await _context.SaveChangesAsync();
        }

        private async Task UpsertLocalizations(int entityPermissionRoleId, List<AdminUpsertLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _context.EntityPermissionRoleLocalizations
                    .FirstOrDefaultAsync(eprl => eprl.EntityPermissionRoleId == entityPermissionRoleId && eprl.LocalizationId == loc.LocalizationId);

                if (existing == null)
                {
                    _context.EntityPermissionRoleLocalizations.Add(new EntityPermissionRoleLocalization
                    {
                        EntityPermissionRoleId = entityPermissionRoleId,
                        LocalizationId = loc.LocalizationId,
                        EntityPermissionRoleDisplayValue = loc.DisplayValue
                    });
                }
                else
                {
                    existing.EntityPermissionRoleDisplayValue = loc.DisplayValue;
                }
            }
            await _context.SaveChangesAsync();
        }

        private static AdminTypeItemDTO MapToDTO(EntityPermissionRole epr) => new()
        {
            Id = epr.EntityPermissionRoleId,
            InternalName = epr.EntityPermissionRoleName,
            Localizations = epr.EntityPermissionRoleLocalizations?.Select(eprl => new AdminLocalizationValueDTO
            {
                LocalizationId = eprl.LocalizationId,
                LocalizationCode = eprl.Localization?.LocalizationCode ?? string.Empty,
                DisplayValue = eprl.EntityPermissionRoleDisplayValue
            }).ToList() ?? new()
        };
    }
}
