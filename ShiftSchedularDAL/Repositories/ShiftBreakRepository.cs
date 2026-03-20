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
    public class ShiftBreakRepository : GenericRepository<ShiftBreak>, IShiftBreakRepository
    {
        private readonly DataContext _context;
        private readonly IUnitOfWork _unitOfWork;
        private readonly DbSet<ShiftBreak> _dbSet;

        #region Constructor

        public ShiftBreakRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _dbSet = context.Set<ShiftBreak>();
        }


        #endregion

        #region Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shiftId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ShiftBreak>> GetBreaksByShiftId(Guid shiftId)
        {
            if (shiftId != Guid.Empty)
            {
                return await _dbSet.Where(i => i.ShiftId.Equals(shiftId)).ToListAsync();
            }
            else
                return null;
        }

        /// <inheritdoc/>
        public async Task<IEnumerable<ShiftBreak>> GetByEntityId(Guid entityId)
        {
            if (entityId == Guid.Empty)
                return Enumerable.Empty<ShiftBreak>();

            return await _dbSet
                .Where(i => i.Shift.EntityId.Equals(entityId))
                .ToListAsync();
        }

        #endregion
    }
}
