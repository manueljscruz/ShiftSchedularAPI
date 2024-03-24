using ShiftSchedularEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IWorkerRepository : IGenericRepository<Worker>
    {
        Task<Worker> GetByEmail(string email);
    }
}
