using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class EntityWorkerInvitationRepository : IEntityWorkerInvitationRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<EntityWorkerInvitation> _dbSet;
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        public EntityWorkerInvitationRepository(DataContext context, IUnitOfWork unitOfWork)
        {
            _context = context;
            _dbSet = context.Set<EntityWorkerInvitation>();
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Add

        public async Task<EntityWorkerInvitation> Add(EntityWorkerInvitation entityWorkerInvitation)
        {
            try
            {
                await _dbSet.AddAsync(entityWorkerInvitation);
                await _unitOfWork.SaveChangesAsync();
                return entityWorkerInvitation;
            }
            catch (Exception ex)
            {
                string test = ex.Message;
            }
            return null;
        }

        #endregion

        #region Delete

        public async Task Delete(EntityWorkerInvitation entityWorkerInvitation)
        {
            EntityWorkerInvitation invitation = await _dbSet.FindAsync(entityWorkerInvitation.EntityId, entityWorkerInvitation.Email);
            if (invitation != null)
            {
                _dbSet.Remove(invitation);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        #endregion

        #region Delete By Composite Key

        public async Task DeleteByCompositeKey(Guid entityId, string email)
        {
            EntityWorkerInvitation invitation = await _dbSet.FindAsync(entityId, email);
            if (invitation != null)
            {
                _dbSet.Remove(invitation);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        #endregion

        #region Delete All By Entity

        public async Task DeleteAllByEntity(Guid entityId)
        {
            IEnumerable<EntityWorkerInvitation> entityWorkerInvitations = _dbSet.Where(i => i.EntityId.Equals(entityId));
            if (entityWorkerInvitations.Count() != 0)
            {
                _dbSet.RemoveRange(entityWorkerInvitations);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        #endregion

        #region Get All By Worker

        public async Task<IEnumerable<EntityWorkerInvitation>> GetAllByWorker(string workerId)
        {
            if (!string.IsNullOrEmpty(workerId))
            {
                return await _dbSet.Where(i => i.ApplicationUserId.Equals(workerId)).ToListAsync();
            }
            else return Enumerable.Empty<EntityWorkerInvitation>();
        }

        #endregion

        #region Get By Entity And Worker

        public async Task<EntityWorkerInvitation?> GetByEntityAndWorker(Guid entityId, string workerId)
        {
            return await _dbSet
                .FirstOrDefaultAsync(i => i.EntityId == entityId && i.ApplicationUserId == workerId);
        }

        #endregion

        #region Get By Composite Key

        public async Task<EntityWorkerInvitation> GetByCompositeKey(Guid entityId, string email, string workerId = null)
        {
            if (string.IsNullOrEmpty(workerId))
            {
                return _dbSet.Where(i => i.EntityId.Equals(entityId) && i.Email.Equals(email)).FirstOrDefault();
            }
            else
            {
                return _dbSet.Where(i => i.EntityId.Equals(entityId) && i.Email.Equals(email) && i.ApplicationUserId.Equals(workerId)).FirstOrDefault();
            }
        }

        #endregion
    }
}
