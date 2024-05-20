using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class ShiftTemplateBreaksRepository : GenericRepository<ShiftTemplateBreaks>, IShiftTemplateBreaksRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<ShiftTemplateBreaks> _shiftTemplateBreaksDbSet;
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        public ShiftTemplateBreaksRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _shiftTemplateBreaksDbSet = _context.Set<ShiftTemplateBreaks>();
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets all shifts templates breaks where the break template id is associated
        /// </summary>
        /// <param name="breakId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ShiftTemplateBreaks>> GetShiftTemplateBreaksByBreakId(int breakId)
        {
            if(breakId != 0)
            {
                return await _shiftTemplateBreaksDbSet.Where(i => i.ShiftBreakTemplateId.Equals(breakId)).ToListAsync();
            }

            return null;
        }

        /// <summary>
        /// Gets all shift templates breaks where the shift template id is associated
        /// </summary>
        /// <param name="shiftId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ShiftTemplateBreaks>> GetShiftTemplateBreaksByShiftId(int shiftId)
        {
            if(shiftId != 0)
            {
                return await _shiftTemplateBreaksDbSet.Where(i => i.ShiftTemplateId.Equals(shiftId)).ToListAsync();
            }

            return null;
        }

        #endregion

        #region Delete Shift Template Break

        public async Task DeleteShiftTemplateBreak(ShiftTemplateBreaks shiftTemplateInstance)
        {
            _shiftTemplateBreaksDbSet.Remove(shiftTemplateInstance);
        }

        #endregion

    }
}
