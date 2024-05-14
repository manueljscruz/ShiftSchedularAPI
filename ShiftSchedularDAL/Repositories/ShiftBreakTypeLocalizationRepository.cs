using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class ShiftBreakTypeLocalizationRepository : GenericRepository<ShiftBreakTypeLocalization>, IShiftBreakTypeLocalizationRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<ShiftBreakTypeLocalization> _shiftBreakTypeLocalizationDbSet;
        private readonly ILocalizationRepository _localizationRepository;
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        public ShiftBreakTypeLocalizationRepository(DataContext context, IUnitOfWork unitOfWork, ILocalizationRepository localizationRepository) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _localizationRepository = localizationRepository;
            _shiftBreakTypeLocalizationDbSet = _context.Set<ShiftBreakTypeLocalization>();
        }

        #endregion

        #region Methods

        #region Get Break Type Localized

        public async Task<ShiftBreakTypeLocalization> GetBreakTypeLocalized(string lcode, int breakTypeId)
        {
            Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(lcode);
            if (localization != null)
                return _shiftBreakTypeLocalizationDbSet.Where(i => i.LocalizationId.Equals(localization.LocalizationId) && i.ShiftBreakTypeId.Equals(breakTypeId)).FirstOrDefault();
            else return null;
        }

        #endregion

        #region Get Shift Breaks Type Localized

        public async Task<IEnumerable<ShiftBreakTypeLocalization>> GetShiftBreaksTypeLocalized(string lcode)
        {
            Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(lcode);
            if (localization != null)
                return await _shiftBreakTypeLocalizationDbSet.Where(i => i.LocalizationId.Equals(localization.LocalizationId)).ToListAsync();
            return null;
        }

        #endregion

        #endregion
    }
}
