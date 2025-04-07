using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class BusinessAspectLocalizationRepository : GenericRepository<BusinessAspect>, IBusinessAspectLocalizationRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<BusinessAspectLocalization> _businessAspectDbSet;
        private readonly ILocalizationRepository _localizationRepository;
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        public BusinessAspectLocalizationRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _unitOfWork = unitOfWork;
            _localizationRepository = unitOfWork.LocalizationRepository;
            _businessAspectDbSet = _context.Set<BusinessAspectLocalization>();
        }

        public Task<BusinessAspectLocalization> Add(BusinessAspectLocalization entity)
        {
            throw new NotImplementedException();
        }

        public Task<List<BusinessAspectLocalization>> AddRange(List<BusinessAspectLocalization> entities)
        {
            throw new NotImplementedException();
        }

        public Task DeleteRange(IEnumerable<BusinessAspectLocalization> entities)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Get Business Aspect Localizations By Id

        public IEnumerable<BusinessAspectLocalization> GetBusinessAspectLocalizationsById(int businessAspectId)
        {
            if (businessAspectId != 0)
            {
                return _businessAspectDbSet.Where(i => i.BusinessAspectId.Equals(businessAspectId));
            }
            else return null;
        }

        #endregion

        #region Get Business Aspects By Localization

        public async Task<IEnumerable<BusinessAspectLocalization>> GetBusinessAspectsByLocalization(string languageCode)
        {
            if (!string.IsNullOrEmpty(languageCode))
            {
                Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(languageCode);
                if (localization != null)
                    return _businessAspectDbSet.Where(i => i.LocalizationId.Equals(localization.LocalizationId));
            }
            else
                return null;
            return null;
        }

        #endregion

        public Task Update(BusinessAspectLocalization entity)
        {
            throw new NotImplementedException();
        }

        public Task UpdateRange(IEnumerable<BusinessAspectLocalization> entities)
        {
            throw new NotImplementedException();
        }

        Task<IEnumerable<BusinessAspectLocalization>> IGenericRepository<BusinessAspectLocalization>.GetAll()
        {
            throw new NotImplementedException();
        }

        Task<BusinessAspectLocalization> IGenericRepository<BusinessAspectLocalization>.GetById(int id)
        {
            throw new NotImplementedException();
        }

        Task<BusinessAspectLocalization> IGenericRepository<BusinessAspectLocalization>.GetById(string id)
        {
            throw new NotImplementedException();
        }

        Task<BusinessAspectLocalization> IGenericRepository<BusinessAspectLocalization>.GetById(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
