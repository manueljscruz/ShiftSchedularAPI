using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.IdentityModel.Tokens;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.APIManagement;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.Service
{
    public class RuleTypeService : IRuleTypeService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILocalizationRepository _localizationRepository;
        private readonly IGenericRepository<RuleType> _ruleTypeRepository;
        private readonly IRuleTypeLocalizationRepository _ruleTypeLocalizationRepository;

        #region Constructor

        public RuleTypeService(IMapper mapper, IUnitOfWork unitOfWork, ILocalizationRepository localizationRepository, IGenericRepository<RuleType> ruleTypeRepository, IRuleTypeLocalizationRepository ruleTypeLocalizationRepository)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _localizationRepository = localizationRepository;
            _ruleTypeRepository = ruleTypeRepository;
            _ruleTypeLocalizationRepository = ruleTypeLocalizationRepository;
        }

        #endregion

        #region Add Rule Type

        /// <summary>
        /// Add a new rule type
        /// </summary>
        /// <param name="strNewRuleType"></param>
        /// <returns></returns>
        public async Task<int> AddRuleType(AddRuleTypeDTO newRuleType)
        {
            int id = 0;
            if (!string.IsNullOrEmpty(newRuleType.NewRuleType))
            {
                RuleType ruleType = new RuleType
                {
                    RuleTypeName = newRuleType.NewRuleType,
                    MultipleSpecification = newRuleType.MultipleSpecification
                };

                ruleType = await _ruleTypeRepository.Add(ruleType);
                id = ruleType.RuleTypeId;
            }
            return id;
        }

        #endregion

        #region Add Rule Type Localization

        /// <summary>
        /// Adds a new localized Rule Type
        /// </summary>
        /// <param name="ruleTypeLocalizationSubmissionModel"></param>
        /// <returns></returns>
        public async Task<bool> AddRuleTypeLocalization(RuleTypeLocalizationSubmissionModel ruleTypeLocalizationSubmissionModel)
        {
            bool result = false;

            if(ruleTypeLocalizationSubmissionModel != null)
            {
                if(ruleTypeLocalizationSubmissionModel.RuleTypeId != 0 && ruleTypeLocalizationSubmissionModel.LanguageId != 0 && !string.IsNullOrEmpty(ruleTypeLocalizationSubmissionModel.DisplayValue))
                {
                    RuleTypeLocalization ruleTypeLocalization = new RuleTypeLocalization
                    {
                        RuleTypeId = ruleTypeLocalizationSubmissionModel.RuleTypeId,
                        LocalizationId = ruleTypeLocalizationSubmissionModel.LanguageId,
                        RuleTypeDisplayValue = ruleTypeLocalizationSubmissionModel.DisplayValue
                    };

                    ruleTypeLocalization = await _ruleTypeLocalizationRepository.Add(ruleTypeLocalization);

                    result = true;
                }
            }

            return result;
        }

        #endregion

        #region Delete Rule Type By Id

        /// <summary>
        /// Deletes all rule types by identifier
        /// Also deletes localization records
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task DeleteRuleTypeById(int id)
        {
            if(id != 0)
            {
                await _unitOfWork.BeginTransactionAsync();
                try
                {
                    IEnumerable<RuleTypeLocalization> rules = _ruleTypeLocalizationRepository.GetRuleTypeLocalizationsByRuleId(id);
                    await _ruleTypeLocalizationRepository.DeleteRange(rules);
                    await _ruleTypeRepository.Delete(id);

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

        #region Get All Rule Types

        /// <summary>
        /// Gets all the rule types
        /// </summary>
        /// <returns></returns>
        public Task<IEnumerable<RuleType>> GetAllRuleTypes()
        {
            return _ruleTypeRepository.GetAll();
        }

        #endregion

        #region Get All Rule Types By Localization

        /// <summary>
        /// Returns a list of all rule types by Localization
        /// </summary>
        /// <param name="lcode"></param>
        /// <returns></returns>
        public async Task<List<RuleTypeLocalizedDTO>> GetAllRuleTypesByLocalization(string lcode)
        {
            List<RuleTypeLocalizedDTO> ruleTypeLocalizedDTOs = new List<RuleTypeLocalizedDTO>();

            if (lcode.Contains("-"))
                lcode = lcode.Split('-')[0];

            IEnumerable<RuleTypeLocalization> ruleTypeLocalizations = await _ruleTypeLocalizationRepository.GetRuleTypesByLocalization(lcode);
            Localization localization = await _localizationRepository.GetLocalizationByLanguageCode(lcode);

            // If data found, add it to list to be returned
            if (localization != null && localization.RuleTypeLocalizations.Count() != 0)
                ruleTypeLocalizedDTOs = localization.RuleTypeLocalizations.AsQueryable().ProjectTo<RuleTypeLocalizedDTO>(_mapper.ConfigurationProvider).ToList();
            
            return ruleTypeLocalizedDTOs;
        }

        #endregion

        #region Get Rule Type By Id

        /// <summary>
        /// Gets rule type by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<RuleType> GetRuleTypeById(int id)
        {
            if (id != 0)
                return await _ruleTypeRepository.GetById(id);
            else return null;
        }

        #endregion

        #region Update Rule Type

        /// <summary>
        /// Updates the rule type instance
        /// </summary>
        /// <param name="ruleType"></param>
        /// <returns></returns>
        public async Task UpdateRuleType(RuleType ruleType)
        {
            if(ruleType != null && !string.IsNullOrEmpty(ruleType.RuleTypeName))
            {
                await _ruleTypeRepository.Update(ruleType);
            }
        }

        #endregion
    }
}
