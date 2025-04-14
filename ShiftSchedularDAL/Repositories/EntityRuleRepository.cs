using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class EntityRuleRepository : GenericRepository<EntityRule>, IEntityRuleRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<EntityRule> _dbSet;
        private readonly IUnitOfWork _unitOfWork;

        public EntityRuleRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _dbSet = _context.Set<EntityRule>();
        }

        public async Task<IEnumerable<EntityRule>> GetEntityRules(Guid entityId)
        {
            if (entityId != Guid.Empty)
            {
                return await _dbSet.Include(x=>x.EntityRuleSpecifications).Where(i => i.EntityId.Equals(entityId)).ToListAsync();
            }
            else return null;
        }
    }
}
