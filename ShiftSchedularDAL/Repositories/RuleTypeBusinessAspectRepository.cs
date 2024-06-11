using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class RuleTypeBusinessAspectRepository : GenericRepository<RuleTypeBusinessAspect>, IRuleTypeBusinessAspectRepository
    {
        private readonly DbContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSet<RuleTypeBusinessAspect> _dbSet;

        public RuleTypeBusinessAspectRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _dbSet = _context.Set<RuleTypeBusinessAspect>();
        }

        public async Task<List<RuleTypeBusinessAspect>> GetRuleTypeBusinessAspectsByRuleTypeId(int ruleTypeId)
        {
            return await _dbSet.Where(i => i.RuleTypeId.Equals(ruleTypeId)).ToListAsync();
        }
    }
}
