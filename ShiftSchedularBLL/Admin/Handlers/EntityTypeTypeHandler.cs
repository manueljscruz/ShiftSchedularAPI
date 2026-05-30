using Microsoft.EntityFrameworkCore;
using ShiftSchedularBLL.Admin;
using ShiftSchedularDAL.Data;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.Admin.Handlers
{
    public class EntityTypeTypeHandler : IAdminTypeHandler
    {
        private readonly DataContext _context;

        public EntityTypeTypeHandler(DataContext context)
        {
            _context = context;
        }

        public bool HasColors => false;

        public async Task<List<AdminTypeItemDTO>> GetAllAsync()
        {
            var items = await _context.EntityTypes
                .Include(et => et.EntityTypeLocalizations)
                    .ThenInclude(etl => etl.Localization)
                .OrderBy(et => et.EntityTypeValue)
                .ToListAsync();

            return items.Select(MapToDTO).ToList();
        }

        public async Task<AdminTypeItemDTO?> GetByIdAsync(int id)
        {
            var item = await _context.EntityTypes
                .Include(et => et.EntityTypeLocalizations)
                    .ThenInclude(etl => etl.Localization)
                .FirstOrDefaultAsync(et => et.EntityTypeId == id);

            return item == null ? null : MapToDTO(item);
        }

        public async Task<int> UpsertAsync(AdminUpsertTypeDTO dto)
        {
            EntityType item;

            if (dto.Id == 0)
            {
                item = new EntityType { EntityTypeValue = dto.InternalName };
                _context.EntityTypes.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                item = await _context.EntityTypes.FindAsync(dto.Id);
                if (item == null) return 0;

                item.EntityTypeValue = dto.InternalName;
                await _context.SaveChangesAsync();
            }

            await UpsertLocalizations(item.EntityTypeId, dto.Localizations);
            return item.EntityTypeId;
        }

        public async Task DeleteAsync(int id)
        {
            var localizations = await _context.EntityTypeLocalizations
                .Where(etl => etl.EntityTypeId == id).ToListAsync();
            _context.EntityTypeLocalizations.RemoveRange(localizations);

            var item = await _context.EntityTypes.FindAsync(id);
            if (item != null) _context.EntityTypes.Remove(item);

            await _context.SaveChangesAsync();
        }

        private async Task UpsertLocalizations(int entityTypeId, List<AdminUpsertLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _context.EntityTypeLocalizations
                    .FirstOrDefaultAsync(etl => etl.EntityTypeId == entityTypeId && etl.LocalizationId == loc.LocalizationId);

                if (existing == null)
                {
                    _context.EntityTypeLocalizations.Add(new EntityTypeLocalization
                    {
                        EntityTypeId = entityTypeId,
                        LocalizationId = loc.LocalizationId,
                        EntityTypeDisplayValue = loc.DisplayValue
                    });
                }
                else
                {
                    existing.EntityTypeDisplayValue = loc.DisplayValue;
                }
            }
            await _context.SaveChangesAsync();
        }

        private static AdminTypeItemDTO MapToDTO(EntityType et) => new()
        {
            Id = et.EntityTypeId,
            InternalName = et.EntityTypeValue,
            Localizations = et.EntityTypeLocalizations?.Select(etl => new AdminLocalizationValueDTO
            {
                LocalizationId = etl.LocalizationId,
                LocalizationCode = etl.Localization?.LocalizationCode ?? string.Empty,
                DisplayValue = etl.EntityTypeDisplayValue
            }).ToList() ?? new()
        };
    }
}
