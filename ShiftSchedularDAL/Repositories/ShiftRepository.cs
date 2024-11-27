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
    public class ShiftRepository : GenericRepository<Shift>, IShiftRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<Shift> _dbSet;
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        public ShiftRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _dbSet = context.Set<Shift>();
        }


        #endregion

        #region Methods


        public async Task<IEnumerable<Shift>> GetEntityShifts(string entityId)
        {
            if (!string.IsNullOrEmpty(entityId))
            {
                return await _dbSet.Where(i => i.EntityId.Equals(entityId)).ToListAsync();
            }
            else return null;
        }

        public async Task<IEnumerable<Shift>> GetEntityShifts(List<string> shiftIdentifiers)
        {
            if (shiftIdentifiers.Count != 0)
            {
                return await _dbSet.Where(i => shiftIdentifiers.Contains(i.ShiftId.ToString())).ToListAsync();
            }
            else return null;
        }

        #endregion
    }
}
