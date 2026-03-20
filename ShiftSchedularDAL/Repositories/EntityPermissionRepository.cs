using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.DbConstants;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularDAL.Repositories
{
    public class EntityPermissionRepository : GenericRepository<EntityPermission>, IEntityPermissionRepository
    {
        private readonly DataContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSet<EntityPermission> _entityPermissionDbSet;

        #region Constructor

        public EntityPermissionRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _entityPermissionDbSet = _context.Set<EntityPermission>();
        }

        #endregion

        #region Can User Create Entities

        public async Task<bool> CanUserCreateEntities(Guid entityId, string workerId)
        {
            return await _entityPermissionDbSet
                .AnyAsync(p => p.EntityId == entityId
                    && p.ApplicationUserId == workerId
                    && (p.EntityPermissionRoleId == EntityPermisisonRoleConstants.GENERAL_MANAGER_ID
                        || (p.EntityPermissionRoleId == EntityPermisisonRoleConstants.MANAGER_ID && p.CanManageChildren)));
        }

        #endregion

        #region Can User Edit Entity

        public async Task<bool> CanUserEditEntity(Guid entityId, string workerId)
        {
            return await _entityPermissionDbSet
                .AnyAsync(p => p.EntityId == entityId
                    && p.ApplicationUserId == workerId
                    && (p.EntityPermissionRoleId == EntityPermisisonRoleConstants.GENERAL_MANAGER_ID
                        || p.EntityPermissionRoleId == EntityPermisisonRoleConstants.MANAGER_ID));
        }

        #endregion

        #region Get By Entity Id

        public async Task<IEnumerable<EntityPermission>> GetByEntityId(Guid entityId)
        {
            return await _entityPermissionDbSet
                .Where(p => p.EntityId == entityId)
                .ToListAsync();
        }

        #endregion

        #region Get By Worker Id

        public async Task<IEnumerable<EntityPermission>> GetByWorkerId(string workerId)
        {
            if (string.IsNullOrEmpty(workerId))
                return Enumerable.Empty<EntityPermission>();

            return await _entityPermissionDbSet
                .Where(p => p.ApplicationUserId == workerId)
                .ToListAsync();
        }

        #endregion

    }
}
