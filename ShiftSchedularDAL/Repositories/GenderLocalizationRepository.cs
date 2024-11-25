using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class GenderLocalizationRepository : GenericRepository<GenderLocalization>, IGenderLocalizationRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<GenderLocalization> _genderLocalizationDbSet;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationRepository _localizationRepository;

        public GenderLocalizationRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _localizationRepository = _unitOfWork.LocalizationRepository;
            _genderLocalizationDbSet = _context.Set<GenderLocalization>();
        }

        public async Task<IEnumerable<GenderLocalization>> GetGendersByLocalization(string languageCode)
        {
            if(string.IsNullOrWhiteSpace(languageCode))
                return Enumerable.Empty<GenderLocalization>();

            Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(languageCode);
            if (localization != null)
                return _genderLocalizationDbSet.Where(i => i.LocalizationId.Equals(localization.LocalizationId));

            else return Enumerable.Empty<GenderLocalization>();
        }
    }
}
