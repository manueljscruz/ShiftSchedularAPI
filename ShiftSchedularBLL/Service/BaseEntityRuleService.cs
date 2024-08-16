using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.APIManagement;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.Service
{
    public class BaseEntityRuleService : IBaseEntityRuleService
    {
        #region Properties

        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<BaseEntityRule> _baseEntityRuleRepository;
        private readonly IBaseEntityRuleSpecificationRepository _baseEntityRuleSpecificationRepository;
        private readonly IRuleTypeService _ruleTypeService;

        #endregion

        #region Constructor

        public BaseEntityRuleService(IMapper mapper, IUnitOfWork unitOfWork, IGenericRepository<BaseEntityRule> baseEntityRuleRepository, IBaseEntityRuleSpecificationRepository baseEntityRuleSpecificationRepository,
            IRuleTypeService ruleTypeService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _baseEntityRuleRepository = baseEntityRuleRepository;
            _baseEntityRuleSpecificationRepository = baseEntityRuleSpecificationRepository;
            _ruleTypeService = ruleTypeService;
        }

        #endregion

        #region Methods

        #region Add Base Entity Rule

        public async Task<int> AddBaseEntityRule(BaseEntityRuleSubmissionModel baseEntityRuleSubmissionModel)
        {
            int newId = 0;

            if (baseEntityRuleSubmissionModel != null && baseEntityRuleSubmissionModel.RuleTypeId != 0)
            {
                RuleType ruleType = await _ruleTypeService.GetRuleTypeById(baseEntityRuleSubmissionModel.RuleTypeId);
                if (ruleType != null)
                {
                    BaseEntityRule baseEntityRule = new BaseEntityRule
                    {
                        RuleTypeId = baseEntityRuleSubmissionModel.RuleTypeId
                    };

                    baseEntityRule = await _baseEntityRuleRepository.Add(baseEntityRule);

                    if (baseEntityRuleSubmissionModel.BaseEntityRuleSpecificationSubmissionModels.Count != 0)
                    {
                        foreach (BaseEntityRuleSpecificationSubmissionModel baseEntityRuleSpecificationSubmissionModel in baseEntityRuleSubmissionModel.BaseEntityRuleSpecificationSubmissionModels)
                        {
                            BaseEntityRuleSpecification baseEntityRuleSpecification = _mapper.Map<BaseEntityRuleSpecification>(baseEntityRuleSpecificationSubmissionModel);
                            baseEntityRuleSpecification = await _baseEntityRuleSpecificationRepository.AddBaseEntityRuleSpecification(baseEntityRuleSpecification);
                        }
                    }

                    newId = baseEntityRule.RuleTypeId;

                }
            }

            return newId;
        }

        #endregion

        #region Add Base Entity Rule Specification

        public async Task<bool> AddBaseEntityRuleSpecification(BaseEntityRuleSpecificationSubmissionModel baseEntityRuleSpecificationSubmissionModel)
        {
            bool result = false;

            if(baseEntityRuleSpecificationSubmissionModel != null)
            {
                BaseEntityRuleSpecification baseEntityRuleSpecification = _mapper.Map<BaseEntityRuleSpecification>(baseEntityRuleSpecificationSubmissionModel);
                baseEntityRuleSpecification = await _baseEntityRuleSpecificationRepository.AddBaseEntityRuleSpecification(baseEntityRuleSpecification);

                result = true;
            }

            return result;
        }

        #endregion

        #region Delete Base Entity Rule By Id

        public async Task<bool> DeleteBaseEntityRuleById(int id)
        {
            bool result = false;
            if(id != 0)
            {
                await _unitOfWork.BeginTransactionAsync();

                try
                {
                    BaseEntityRule baseEntityRule = await _baseEntityRuleRepository.GetById(id);
                    if(baseEntityRule != null)
                    {
                        bool removeSpecs = await _baseEntityRuleSpecificationRepository.DeleteAllBaseEntityRuleSpecificationsById(id);
                        if (!removeSpecs)
                        {
                            await _unitOfWork.RollbackAsync();
                            return result;
                        }

                        await _baseEntityRuleRepository.Delete(baseEntityRule.BaseEntityRuleId);
                        await _unitOfWork.SaveChangesAsync();
                        result = true;
                    }
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

            return result;
        }

        #endregion

        #region Delete Base Entity Rule Specification

        public async Task<bool> DeleteBaseEntityRuleSpecification(int baseRuleId, int specId)
        {
            bool result = false;

            if (baseRuleId != 0 && specId != 0)
                result = await _baseEntityRuleSpecificationRepository.DeleteBaseEntityRuleSpecification(baseRuleId, specId);

            return result;
        }

        #endregion

        #region Get Base Entity Rule By Id

        public async Task<BaseEntityRule> GetBaseEntityRuleById(int id)
        {
            if (id != 0)
                return await _baseEntityRuleRepository.GetById(id);

            return null;
        }

        #endregion

        #region Get Base Entity Rules

        public async Task<List<BaseEntityRuleDTO>> GetBaseEntityRules(string lcode)
        {
            List<BaseEntityRuleDTO> baseEntityRuleDTOs = new List<BaseEntityRuleDTO>();

            if (!string.IsNullOrEmpty(lcode))
            {
                IEnumerable<BaseEntityRule> baseEntityRules = await _baseEntityRuleRepository.GetAll();
                List<RuleTypeLocalizedDTO> ruleTypeLocalizedDTOs = await _ruleTypeService.GetAllRuleTypesByLocalization(lcode);

                foreach (BaseEntityRule baseEntityRule in baseEntityRules)
                {
                    BaseEntityRuleDTO baseEntityRuleDTO = _mapper.Map<BaseEntityRuleDTO>(baseEntityRule);
                    baseEntityRuleDTO.RuleTypeDisplayValue = ruleTypeLocalizedDTOs.Where(i => i.RuleTypeId.Equals(baseEntityRule.RuleTypeId)).FirstOrDefault().RuleTypeLocalizedName ?? "";
                    baseEntityRuleDTO.BaseEntityRuleSpecifications = (List<BaseEntityRuleSpecification>)await _baseEntityRuleSpecificationRepository.GetRuleSpecificationsById(baseEntityRule.BaseEntityRuleId);

                    baseEntityRuleDTOs.Add(baseEntityRuleDTO);
                }
            }

            return baseEntityRuleDTOs;
        }

        #endregion

        #region Update Base Entity Rule

        public async Task<bool> UpdateBaseEntityRule(BaseEntityRule baseEntityRule)
        {
            bool result = false;

            if(baseEntityRule != null)
            {
                await _baseEntityRuleRepository.Update(baseEntityRule);
                result = true;
            }

            return result;
        }

        #endregion

        #region Update Base Entity Rule

        public async Task<bool> UpdateBaseEntityRuleSpecification(BaseEntityRuleSpecification baseEntityRuleSpecification)
        {
            bool result = false;

            if(baseEntityRuleSpecification != null)
            {
                if(await _baseEntityRuleSpecificationRepository.UpdateBaseEntityRuleSpecification(baseEntityRuleSpecification))
                    result = true;
            }

            return result;
        }

        #endregion

        #endregion
    }
}
