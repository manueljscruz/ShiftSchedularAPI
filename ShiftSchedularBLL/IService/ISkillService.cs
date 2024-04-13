using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.API_Management;
using ShiftSchedularEntity.Models.DataTransferObjects;

namespace ShiftSchedularBLL.IService
{
    public interface ISkillService
    {
        Task<Skill> GetSkillById(int skillId);
        Task<IEnumerable<Skill>> GetAllSkills();
        Task<int> AddSkill(SkillSubmissionModel newSkillSubModel);
        Task UpdateSkill(Skill skill);
        Task DeleteSkill(int skillId);
        Task<bool> AddSkillLocalization(SkillLocalizationSubmissionModel skillLocalizationSubmission);
        Task<List<SkillLocalizedDTO>> GetAllSkillsByLocalization(string lcode);
    }
}
