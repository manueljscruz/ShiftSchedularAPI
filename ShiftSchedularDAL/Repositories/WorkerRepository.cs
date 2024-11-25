using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
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
    public class WorkerRepository : GenericRepository<Worker>, IWorkerRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<Worker> _workerDbSet;
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        public WorkerRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _workerDbSet = _context.Set<Worker>();
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Get Worker By Email

        /// <summary>
        /// Gets an worker entry based on the email
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Worker> GetByEmail(string email)
        {
            if (!string.IsNullOrEmpty(email))
            {
                return await _workerDbSet.Where(x => x.Email == email).AsNoTracking().FirstOrDefaultAsync();
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
    
}
