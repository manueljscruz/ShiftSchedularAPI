using Microsoft.EntityFrameworkCore;
using ShiftSchedularBLL.Admin;
using ShiftSchedularDAL.Data;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularBLL.Admin.Handlers
{
    public class SkillTypeHandler : IAdminTypeHandler
    {
        private readonly DataContext _context;

        public SkillTypeHandler(DataContext context)
        {
            _context = context;
        }

        public bool HasColors => true;

        public async Task<List<AdminTypeItemDTO>> GetAllAsync()
        {
            var skills = await _context.Skills
                .Include(s => s.SkillLocalizations)
                    .ThenInclude(sl => sl.Localization)
                .OrderBy(s => s.SkillName)
                .ToListAsync();

            return skills.Select(MapToDTO).ToList();
        }

        public async Task<AdminTypeItemDTO?> GetByIdAsync(int id)
        {
            var skill = await _context.Skills
                .Include(s => s.SkillLocalizations)
                    .ThenInclude(sl => sl.Localization)
                .FirstOrDefaultAsync(s => s.SkillId == id);

            return skill == null ? null : MapToDTO(skill);
        }

        public async Task<int> UpsertAsync(AdminUpsertTypeDTO dto)
        {
            Skill skill;

            if (dto.Id == 0)
            {
                skill = new Skill
                {
                    SkillName = dto.InternalName,
                    HexBGColor = dto.HexBGColor ?? "#FFFFFF",
                    HexFontColor = dto.HexFontColor ?? "#000000"
                };
                _context.Skills.Add(skill);
                await _context.SaveChangesAsync();
            }
            else
            {
                skill = await _context.Skills.FindAsync(dto.Id);
                if (skill == null) return 0;

                skill.SkillName = dto.InternalName;
                skill.HexBGColor = dto.HexBGColor ?? skill.HexBGColor;
                skill.HexFontColor = dto.HexFontColor ?? skill.HexFontColor;
                await _context.SaveChangesAsync();
            }

            await UpsertLocalizations(skill.SkillId, dto.Localizations);
            return skill.SkillId;
        }

        public async Task DeleteAsync(int id)
        {
            var localizations = await _context.SkillLocalizations
                .Where(sl => sl.SkillId == id).ToListAsync();
            _context.SkillLocalizations.RemoveRange(localizations);

            var skill = await _context.Skills.FindAsync(id);
            if (skill != null) _context.Skills.Remove(skill);

            await _context.SaveChangesAsync();
        }

        private async Task UpsertLocalizations(int skillId, List<AdminUpsertLocalizationDTO> localizations)
        {
            foreach (var loc in localizations)
            {
                var existing = await _context.SkillLocalizations
                    .FirstOrDefaultAsync(sl => sl.SkillId == skillId && sl.LocalizationId == loc.LocalizationId);

                if (existing == null)
                {
                    _context.SkillLocalizations.Add(new SkillLocalization
                    {
                        SkillId = skillId,
                        LocalizationId = loc.LocalizationId,
                        SkillDisplayValue = loc.DisplayValue
                    });
                }
                else
                {
                    existing.SkillDisplayValue = loc.DisplayValue;
                }
            }
            await _context.SaveChangesAsync();
        }

        private static AdminTypeItemDTO MapToDTO(Skill skill) => new()
        {
            Id = skill.SkillId,
            InternalName = skill.SkillName,
            HexBGColor = skill.HexBGColor,
            HexFontColor = skill.HexFontColor,
            Localizations = skill.SkillLocalizations?.Select(sl => new AdminLocalizationValueDTO
            {
                LocalizationId = sl.LocalizationId,
                LocalizationCode = sl.Localization?.LocalizationCode ?? string.Empty,
                DisplayValue = sl.SkillDisplayValue
            }).ToList() ?? new()
        };
    }
}
