using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.DbConstants;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
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

        #region Is General Manager

        public async Task<bool> IsGeneralManager(Guid entityId, string workerId)
        {
            return await _entityPermissionDbSet
                .AnyAsync(p => p.EntityId == entityId
                    && p.ApplicationUserId == workerId
                    && p.EntityPermissionRoleId == EntityPermisisonRoleConstants.GENERAL_MANAGER_ID);
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

        #region Get By Entity And Worker

        public async Task<EntityPermission?> GetByEntityAndWorker(Guid entityId, string workerId)
        {
            byte[] entityIdBytes = entityId.ToByteArray();
            return await _entityPermissionDbSet
                .FromSqlRaw(
                    "SELECT * FROM [dbo].[EntityPermissions] WHERE EntityId = @entityId AND ApplicationUserId = @workerId AND IsDeleted = 0",
                    new SqlParameter("@entityId", SqlDbType.Binary) { Value = entityIdBytes, Size = 16 },
                    new SqlParameter("@workerId", workerId))
                .FirstOrDefaultAsync();
        }

        #endregion

        #region Find By Entity And Worker (includes soft-deleted)

        public async Task<EntityPermission?> FindByEntityAndWorker(Guid entityId, string workerId)
        {
            byte[] entityIdBytes = entityId.ToByteArray();
            return await _entityPermissionDbSet
                .FromSqlRaw(
                    "SELECT * FROM [dbo].[EntityPermissions] WHERE EntityId = @entityId AND ApplicationUserId = @workerId",
                    new SqlParameter("@entityId", SqlDbType.Binary) { Value = entityIdBytes, Size = 16 },
                    new SqlParameter("@workerId", workerId))
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync();
        }

        #endregion

        #region Find By Entity, Worker And Role (includes soft-deleted)

        public async Task<EntityPermission?> FindByEntityWorkerAndRole(Guid entityId, string workerId, int roleId)
        {
            byte[] entityIdBytes = entityId.ToByteArray();
            return await _entityPermissionDbSet
                .FromSqlRaw(
                    "SELECT * FROM [dbo].[EntityPermissions] WHERE EntityId = @entityId AND ApplicationUserId = @workerId AND EntityPermissionRoleId = @roleId",
                    new SqlParameter("@entityId", SqlDbType.Binary) { Value = entityIdBytes, Size = 16 },
                    new SqlParameter("@workerId", workerId),
                    new SqlParameter("@roleId", roleId))
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync();
        }

        #endregion

        #region Delete By Entity And Worker

        public async Task DeleteByEntityAndWorker(Guid entityId, string workerId)
        {
            byte[] entityIdBytes = entityId.ToByteArray();
            EntityPermission permission = await _entityPermissionDbSet
                .FromSqlRaw(
                    "SELECT * FROM [dbo].[EntityPermissions] WHERE EntityId = @entityId AND ApplicationUserId = @workerId AND IsDeleted = 0",
                    new SqlParameter("@entityId", SqlDbType.Binary) { Value = entityIdBytes, Size = 16 },
                    new SqlParameter("@workerId", workerId))
                .FirstOrDefaultAsync();
            if (permission != null)
            {
                _entityPermissionDbSet.Remove(permission);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        #endregion

    }
}
