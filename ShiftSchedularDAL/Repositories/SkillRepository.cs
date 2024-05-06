using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class SkillRepository : GenericRepository<Skill>, ISkillRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<Skill> _skillDbSet;
        private readonly IUnitOfWork _unitOfWork;

        public SkillRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _skillDbSet = _context.Set<Skill>();
            _unitOfWork = unitOfWork;
        }

        public async Task<List<Skill>> GetSkillsByNames(List<string> skillNames)
        {
            List<Skill> skills = _context.Skills.Where(s => skillNames.Contains(s.SkillName)).ToList();
            return skills;
        }
    }
}
