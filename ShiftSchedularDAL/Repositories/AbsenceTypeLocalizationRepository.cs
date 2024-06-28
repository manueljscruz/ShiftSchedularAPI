using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class AbsenceTypeLocalizationRepository : GenericRepository<AbsenceTypeLocalization>, IAbsenceTypeLocalizationRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<AbsenceTypeLocalization> _absenceTypeLocalizationDbSet;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationRepository _localizationRepository;

        #region Constructor

        public AbsenceTypeLocalizationRepository(DataContext context, IUnitOfWork unitOfWork, ILocalizationRepository localizationRepository) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _localizationRepository = localizationRepository;
            _absenceTypeLocalizationDbSet = _context.Set<AbsenceTypeLocalization>();
        }

        #endregion

        #region Get Absence Type Localizations By Id

        /// <summary>
        /// Gets absence types by absence type id
        /// </summary>
        /// <param name="absenceTypeId"></param>
        /// <returns></returns>
        public IEnumerable<AbsenceTypeLocalization> GetAbsenceTypeLocalizationsById(int absenceTypeId)
        {
            if (absenceTypeId != 0)
            {
                return _absenceTypeLocalizationDbSet.Where(i => i.AbsenceTypeId.Equals(absenceTypeId));
            }
            else return null;
        }

        #endregion

        #region Get Absence Types By Localization

        /// <summary>
        /// Gets absence types by localization code
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        public async Task<IEnumerable<AbsenceTypeLocalization>> GetAbsenceTypesByLocalization(string languageCode)
        {
            if (!string.IsNullOrEmpty(languageCode))
            {
                Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(languageCode);
                if (localization != null)
                    return _absenceTypeLocalizationDbSet.Where(i => i.LocalizationId.Equals(localization.LocalizationId));
            }
            else
                return null;
            return null;
        }

        #endregion
    }
}
