using ShiftSchedularEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityPermissionRepository : IGenericRepository<EntityPermission>
    {
        Task<bool> CanUserCreateEntities(Guid entityId, string workerId);
        Task<IEnumerable<EntityPermission>> GetByEntityId(Guid entityId);
    }
}
