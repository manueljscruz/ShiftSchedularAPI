using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IEntityWorkerRepository : IGenericRepository<EntityWorker>
    {
        Task<IEnumerable<EntityWorkerDTO>> GetByWorkerId(string workerId);
    }
}
