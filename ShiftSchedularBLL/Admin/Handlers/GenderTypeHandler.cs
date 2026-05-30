using Microsoft.EntityFrameworkCore;
using ShiftSchedularBLL.Admin;
using ShiftSchedularDAL.Data;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.Admin.Handlers
{
    public class GenderTypeHandler : IAdminTypeHandler
    {
        private readonly DataContext _context;

        public GenderTypeHandler(DataContext context)
        {
            _context = context;
        }

        public bool HasColors => false;

        public async Task<List<AdminTypeItemDTO>> GetAllAsync()
        {
            var items = await _context.Genders
                .Include(g => g.GenderLocalizations)
                    .ThenInclude(gl => gl.Localization)
                .OrderBy(g => g.GenderValue)
                .ToListAsync();

            return items.Select(MapToDTO).ToList();
        }

        public async Task<AdminTypeItemDTO?> GetByIdAsync(int id)
        {
            var item = await _context.Genders
                .Include(g => g.GenderLocalizations)
                    .ThenInclude(gl => gl.Localization)
                .FirstOrDefaultAsync(g => g.GenderId == id);

            return item == null ? null : MapToDTO(item);
        }

        public async Task<int> UpsertAsync(AdminUpsertTypeDTO dto)
        {
            Gender item;

            if (dto.Id == 0)
            {
                item = new Gender { GenderValue = dto.InternalName };
                _context.Genders.Add(item);
                await _context.SaveChangesAsync();
            }
            else
            {
                item = await _context.Genders.FindAsync(dto.Id);
                if (item == null) return 0;

                item.GenderValue = dto.InternalName;
                await _context.SaveChangesAsync();
            }

            await UpsertLocalizations(item.GenderId, dto.Localizations);
            return item.GenderId;
        }

        public async Task DeleteAsync(int id)
        {
            var localizations = await _context.GenderLocalizations
                .Where(gl => gl.GenderId == id).ToListAsync();
            _context.GenderLocalizations.RemoveRange(localizations);

            var item = await _context.Genders.FindAsync(id);
            if (item != null) _context.Genders.Remove(item);

            await _context.SaveChangesAsync();
        }

        private async Task UpsertLocalizations(int genderId, List<AdminUpsertLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _context.GenderLocalizations
                    .FirstOrDefaultAsync(gl => gl.GenderId == genderId && gl.LocalizationId == loc.LocalizationId);

                if (existing == null)
                {
                    _context.GenderLocalizations.Add(new GenderLocalization
                    {
                        GenderId = genderId,
                        LocalizationId = loc.LocalizationId,
                        GenderDisplayValue = loc.DisplayValue
                    });
                }
                else
                {
                    existing.GenderDisplayValue = loc.DisplayValue;
                }
            }
            await _context.SaveChangesAsync();
        }

        private static AdminTypeItemDTO MapToDTO(Gender g) => new()
        {
            Id = g.GenderId,
            InternalName = g.GenderValue,
            Localizations = g.GenderLocalizations?.Select(gl => new AdminLocalizationValueDTO
            {
                LocalizationId = gl.LocalizationId,
                LocalizationCode = gl.Localization?.LocalizationCode ?? string.Empty,
                DisplayValue = gl.GenderDisplayValue
            }).ToList() ?? new()
        };
    }
}
