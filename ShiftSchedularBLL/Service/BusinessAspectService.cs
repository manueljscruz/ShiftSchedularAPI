using AutoMapper;
using AutoMapper.QueryableExtensions;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.APIManagement;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.Service
{
    public class BusinessAspectService : IBusinessAspectService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationRepository _localizationRepository;
        private readonly IGenericRepository<BusinessAspect> _businessAspectRepository;
        private readonly IBusinessAspectLocalizationRepository _businessAspectLocalizationRepository;

        public BusinessAspectService(IMapper mapper, IUnitOfWork unitOfWork, ILocalizationRepository localizationRepository, IGenericRepository<BusinessAspect> businessAspectRepository, IBusinessAspectLocalizationRepository businessAspectLocalizationRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _localizationRepository = localizationRepository;
            _businessAspectRepository = businessAspectRepository;
            _businessAspectLocalizationRepository = businessAspectLocalizationRepository;
        }

        #region Add Business Aspect

        /// <summary>
        /// Adds a new business aspect
        /// </summary>
        /// <param name="strNewBusinessAspect"></param>
        /// <returns></returns>
        public async Task<int> AddBusinessAspect(string strNewBusinessAspect)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(strNewBusinessAspect))
            {
                BusinessAspect businessAspect = new BusinessAspect
                {
                    BusinessAspectName = strNewBusinessAspect
                };

                businessAspect = await _businessAspectRepository.Add(businessAspect);
                id = businessAspect.BusinessAspectId;
            }
            return id;
        }

        #endregion

        #region Add Business Aspect Localization

        /// <summary>
        /// Adds a new localized Business Aspect
        /// </summary>
        /// <param name="submissionModel"></param>
        /// <returns></returns>
        public async Task<bool> AddBusinessAspectLocalization(BusinessAspectLocalizationSubmissionModel submissionModel)
        {
            bool result = false;

            if (submissionModel != null)
            {
                if (submissionModel.BusinessAspectId != 0 && submissionModel.LanguageId != 0 && !string.IsNullOrEmpty(submissionModel.DisplayValue))
                {
                    BusinessAspectLocalization businessAspectLocalization = new BusinessAspectLocalization
                    {
                        BusinessAspectId = submissionModel.BusinessAspectId,
                        LocalizationId = submissionModel.LanguageId,
                        BusinessAspectDisplayValue = submissionModel.DisplayValue
                    };

                    businessAspectLocalization = await _businessAspectLocalizationRepository.Add(businessAspectLocalization);

                    result = true;
                }
            }

            return result;
        }

        #endregion

        #region Delete Business Aspect By Id

        /// <summary>
        /// Deletes Business Aspect by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteBusinessAspectById(int id)
        {
            if (id != 0)
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    IEnumerable<BusinessAspectLocalization> businessAspects = _businessAspectLocalizationRepository.GetBusinessAspectLocalizationsById(id);
                    await _businessAspectLocalizationRepository.DeleteRange(businessAspects);
                    await _businessAspectRepository.Delete(id);

                    await _unitOfWork.CommitAsync();
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                }
                finally
                {
                    _unitOfWork.Dispose();
                }
            }
        }

        #endregion

        #region Get All Business Aspects

        /// <summary>
        /// Gets all Business Aspects instances
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<BusinessAspect>> GetAllBusinessAspects()
        {
            return _businessAspectRepository.GetAll();
        }

        #endregion

        #region Get All Business Aspects By Localization

        /// <summary>
        /// Gets all localized business aspects
        /// </summary>
        /// <param name="lcode"></param>
        /// <returns></returns>
        public async Task<List<BusinessAspectLocalizedDTO>> GetAllBusinessAspectsByLocalization(string lcode)
        {
            List<BusinessAspectLocalizedDTO> businessAspectLocalizedDTOs = new List<BusinessAspectLocalizedDTO>();

            if (lcode.Contains("-"))
                lcode = lcode.Split('-')[0];

            IEnumerable<BusinessAspectLocalization> businessAspectLocalizations = await _businessAspectLocalizationRepository.GetBusinessAspectsByLocalization(lcode);
            Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(lcode);

            // If data found, add it to list to be returned
            if (localization != null && localization.BusinessAspectLocalizations.Count() != 0)
                businessAspectLocalizedDTOs = localization.BusinessAspectLocalizations.AsQueryable().ProjectTo<BusinessAspectLocalizedDTO>(_mapper.ConfigurationProvider).ToList();

            return businessAspectLocalizedDTOs;
        }

        #endregion

        #region Get Business Aspect By Id

        /// <summary>
        /// Gets Business Aspect identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<BusinessAspect> GetBusinessAspectById(int id)
        {
            if (id != 0)
                return await _businessAspectRepository.GetById(id);
            else return null;
        }

        #endregion

        #region Update Business Aspect

        /// <summary>
        /// Updates the Business Aspect instance
        /// </summary>
        /// <param name="businessAspect"></param>
        /// <returns></returns>
        public async Task UpdateBusinessAspect(BusinessAspect businessAspect)
        {
            if (businessAspect != null && !string.IsNullOrEmpty(businessAspect.BusinessAspectName))
            {
                await _businessAspectRepository.Update(businessAspect);
            }
        }

        #endregion
    }
}
