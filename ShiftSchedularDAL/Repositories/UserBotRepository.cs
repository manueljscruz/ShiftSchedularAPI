using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class UserBotRepository : GenericRepository<UserBot>, IUserBotRepository
    {
        private readonly DataContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSet<UserBot> _dbSet;

        public UserBotRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _dbSet = context.Set<UserBot>();
        }

        public async Task<IEnumerable<UserBot>> GetUserBotsByEntityId(Guid entityId)
        {
            if(string.IsNullOrEmpty(entityId.ToString()))
            {
                return null;
            }
            else
                return await _dbSet.Where(i => i.EntityId.Equals(entityId)).ToListAsync();
        }
    }
}
