using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface ISkillLocalizationRepository : IGenericRepository<SkillLocalization>
    {
        Task<SkillLocalization> GetSkillByCodeAndId(int skillId, string lcode);
    }
}
