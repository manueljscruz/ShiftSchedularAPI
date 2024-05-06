using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface ISkillRepository : IGenericRepository<Skill>
    {
        Task<List<Skill>> GetSkillsByNames(List<string> skillNames);
    }
}
