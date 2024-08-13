using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.DbConstants;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.EntityRuleManagement;

namespace ShiftSchedularBLL.Service
{
    public class EntityRuleService : IEntityRuleService
    {
        private readonly IMapper _mapper;
        private readonly IGeneralService _generalService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Entity> _entityRepository;
        private readonly ISkillService _skillService;
        private readonly IEntityRuleRepository _entityRuleRepository;
        private readonly IEntityWorkerRepository _entityWorkerRepository;
        private readonly IEntityRuleSpecificationRepository _entityRuleSpecificationRepository;
        private readonly IGenericRepository<RuleType> _ruleTypeRepository;
        private readonly IRuleTypeLocalizationRepository _ruleTypeLocalizationRepository;
        private readonly IGenericRepository<BusinessAspect> _bussinessAspectRepository;
        private readonly IBusinessAspectLocalizationRepository _businessAspectLocalizationRepository;
        private readonly IRuleTypeBusinessAspectRepository _ruleTypeBusinessAspectRepository;
        private readonly IShiftService _shiftService;

        #region Constructor

        public EntityRuleService(
            IMapper mapper,
            IGeneralService generalService,
            IUnitOfWork unitOfWork,
            IGenericRepository<Entity> entityRepository,
            ISkillService skillService,
            IEntityRuleRepository entityRuleRepository,
            IEntityWorkerRepository entityWorkerRepository,
            IEntityRuleSpecificationRepository entityRuleSpecificationRepository,
            IGenericRepository<RuleType> ruleTypeRepository,
            IRuleTypeLocalizationRepository ruleTypeLocalizationRepository,
            IGenericRepository<BusinessAspect> businessAspectRepository,
            IBusinessAspectLocalizationRepository businessAspectLocalizationRepository,
            IRuleTypeBusinessAspectRepository ruleTypeBusinessAspectRepository,
            IShiftService shiftService)
        {
            _mapper = mapper;
            _generalService = generalService;
            _unitOfWork = unitOfWork;
            _entityRepository = entityRepository;
            _skillService = skillService;
            _entityRuleRepository = entityRuleRepository;
            _entityWorkerRepository = entityWorkerRepository;
            _entityRuleSpecificationRepository = entityRuleSpecificationRepository;
            _ruleTypeRepository = ruleTypeRepository;
            _bussinessAspectRepository = businessAspectRepository;
            _ruleTypeLocalizationRepository = ruleTypeLocalizationRepository;
            _businessAspectLocalizationRepository = businessAspectLocalizationRepository;
            _ruleTypeBusinessAspectRepository = ruleTypeBusinessAspectRepository;
            _shiftService = shiftService;
        }

        #endregion

        #region Methods

        #region Add Entity Rule

        public async Task<BaseResponse<EntityRuleDTO>> AddEntityRule(AddEntityRuleDTO addEntityRuleDTO)
        {
            BaseResponse<EntityRuleDTO> response = new BaseResponse<EntityRuleDTO>();
            response.Success = false;
            response.Message = EntityRulesRelatedMessages.AddEntityRuleUnexpectedError;

            if (addEntityRuleDTO != null)
            {
                if (addEntityRuleDTO.RuleTypeId == 0)
                {
                    response.Message = EntityRulesRelatedMessages.AddNewEntityRuleTypeIsZero;
                    return response;
                }

                else if (string.IsNullOrEmpty(addEntityRuleDTO.EntityId))
                {
                    response.Message = EntityRulesRelatedMessages.AddNewEntityEntityIdEmpty;
                    return response;
                }

                else if (addEntityRuleDTO.EntityRuleSpecifications.Count() == 0)
                {
                    response.Message = EntityRulesRelatedMessages.AddNewEntityNoSpecificationsFound;
                    return response;
                }

                // Get entity and check ifs null
                Entity destinationEntity = await _entityRepository.GetById(addEntityRuleDTO.EntityId);
                if (destinationEntity == null)
                {
                    response.Message = EntityRulesRelatedMessages.AddNewEntityRuleEntityNotFound;
                    return response;
                }
                else
                {
                    await _unitOfWork.BeginTransactionAsync();

                    try
                    {

                        EntityRule newRule = _mapper.Map<EntityRule>(addEntityRuleDTO);
                        newRule.EntityRuleId = _generalService.GenerateGuid();

                        // Clear the list due to mapping
                        if (newRule.EntityRuleSpecifications.Count != 0)
                            newRule.EntityRuleSpecifications = new List<EntityRuleSpecification>();

                        await _entityRuleRepository.Add(newRule);

                        EntityRuleDTO entityRuleDTO = _mapper.Map<EntityRuleDTO>(newRule);

                        if (addEntityRuleDTO.EntityRuleSpecifications.Count != 0)
                        {
                            foreach (AddEntityRuleSpecificationDTO specification in addEntityRuleDTO.EntityRuleSpecifications)
                            {
                                specification.EntityRuleId = newRule.EntityRuleId;
                                BaseResponse<EntityRuleSpecificationDTO> ruleSpecificationResponse = await this.AddEntityRuleSpecification(specification);
                                if (!ruleSpecificationResponse.Success)
                                {
                                    response.Message = ruleSpecificationResponse.Message;
                                    await _unitOfWork.RollbackAsync();
                                    return response;
                                }
                                else
                                {
                                    entityRuleDTO.EntityRuleSpecificationDTOs.Add(ruleSpecificationResponse.Result);
                                }
                            }
                        }

                        await _unitOfWork.CommitAsync();
                        response.Success = true;
                        response.Result = entityRuleDTO;
                        response.Message = EntityRulesRelatedMessages.AddEntityRuleSuccessful;
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

            return response;
        }

        #endregion

        #region Add Entity Rule Specification

        public async Task<BaseResponse<EntityRuleSpecificationDTO>> AddEntityRuleSpecification(AddEntityRuleSpecificationDTO addEntityRuleSpecificationDTO)
        {
            BaseResponse<EntityRuleSpecificationDTO> response = new BaseResponse<EntityRuleSpecificationDTO>();
            response.Success = false;
            response.Message = EntityRulesRelatedMessages.AddEntityRuleSpecUnexpectedError;

            if (addEntityRuleSpecificationDTO != null)
            {
                // If the entity rule identifier is empty, send error
                if (string.IsNullOrEmpty(addEntityRuleSpecificationDTO.EntityRuleId))
                {
                    response.Message = EntityRulesRelatedMessages.AddEntityRuleSpecEntityRuleIsEmpty;
                    return response;
                }

                // Map rule specification and add it to the database
                EntityRuleSpecification ruleSpecification = _mapper.Map<EntityRuleSpecification>(addEntityRuleSpecificationDTO);
                ruleSpecification = await _entityRuleSpecificationRepository.AddEntityRuleSpecification(ruleSpecification);

                EntityRuleSpecificationDTO entityRuleSpecification = _mapper.Map<EntityRuleSpecificationDTO>(ruleSpecification);
                entityRuleSpecification = await HandleEntityRuleSpecReferences(entityRuleSpecification, addEntityRuleSpecificationDTO.LanguageCode);

                // Return success results
                response.Result = entityRuleSpecification;
                response.Success = true;
                response.Message = EntityRulesRelatedMessages.AddEntityRuleSpecSuccessful;
            }

            return response;
        }

        #endregion

        #region Handle Entity Rule Spec References

        private async Task<EntityRuleSpecificationDTO> HandleEntityRuleSpecReferences(EntityRuleSpecificationDTO entityRuleSpecificationDTO, string lcode)
        {
            if (!string.IsNullOrEmpty(entityRuleSpecificationDTO.AspectReferenceId) && entityRuleSpecificationDTO.BusinessAspectId != 0)
            {
                BusinessAspect businessAspect = await _bussinessAspectRepository.GetById(entityRuleSpecificationDTO.BusinessAspectId);
                if (businessAspect != null && businessAspect.BusinessAspectName.Equals(BusinessAspectsConstants.SHIFTS))
                {
                    ShiftDTO shiftDTO = await _shiftService.GetShiftById(entityRuleSpecificationDTO.AspectReferenceId, LocalizationConstants.ENGLISH);
                    entityRuleSpecificationDTO.ReferenceName = !string.IsNullOrEmpty(shiftDTO.ShiftName) ? shiftDTO.ShiftName : "";
                }
                else if (businessAspect != null && businessAspect.BusinessAspectName.Equals(BusinessAspectsConstants.SKILLS))
                {
                    SkillLocalizedDTO skillLocalizedDTO = await _skillService.GetSkillLocalized(int.Parse(entityRuleSpecificationDTO.AspectReferenceId), lcode);
                    entityRuleSpecificationDTO.ReferenceName = skillLocalizedDTO != null ? skillLocalizedDTO.SkillLocalizedName : "";
                }
            }

            if (!string.IsNullOrEmpty(entityRuleSpecificationDTO.AspectReferenceId2) && entityRuleSpecificationDTO.BusinessAspectId2 != 0)
            {
                BusinessAspect businessAspect = await _bussinessAspectRepository.GetById(entityRuleSpecificationDTO.BusinessAspectId2);
                if (businessAspect != null && businessAspect.BusinessAspectName.Equals(BusinessAspectsConstants.SHIFTS))
                {
                    ShiftDTO shiftDTO = await _shiftService.GetShiftById(entityRuleSpecificationDTO.AspectReferenceId2, LocalizationConstants.ENGLISH);
                    entityRuleSpecificationDTO.ReferenceName2 = !string.IsNullOrEmpty(shiftDTO.ShiftName) ? shiftDTO.ShiftName : "";
                }
                else if (businessAspect != null && businessAspect.BusinessAspectName.Equals(BusinessAspectsConstants.SKILLS))
                {
                    SkillLocalizedDTO skillLocalizedDTO = await _skillService.GetSkillLocalized(int.Parse(entityRuleSpecificationDTO.AspectReferenceId2), lcode);
                    entityRuleSpecificationDTO.ReferenceName2 = skillLocalizedDTO != null ? skillLocalizedDTO.SkillLocalizedName : "";
                }
            }

            return entityRuleSpecificationDTO;
        }

        #endregion

        #region Delete Entity Rule

        /// <summary>
        /// Deletes the entity rule and any specifications associated with
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="entityRuleId"></param>
        /// <returns></returns>
        public async Task<BaseResponse<bool>> DeleteEntityRule(string entityId, string entityRuleId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = "";

            // If entity owner is empty
            if (string.IsNullOrEmpty(entityId))
            {
                response.Message = EntityRulesRelatedMessages.EntityRuleEntityIdentifierIsEmpty;
                return response;
            }

            // entity rule identifier is empty
            else if (string.IsNullOrEmpty(entityRuleId))
            {
                response.Message = EntityRulesRelatedMessages.EntityRuleIdentifierIsEmpty;
                return response;
            }

            // Get entity rule instance from the database
            EntityRule entityRule = await _entityRuleRepository.GetById(entityRuleId);
            if (entityRule == null)
            {
                response.Message = EntityRulesRelatedMessages.EntityRuleNotFound;
                return response;
            }

            // Start transaction 
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Delete all entity rule specifications associated to this entity rule
                bool specsDeleteResult = await _entityRuleSpecificationRepository.DeleteEntityRuleSpecificationsByRuleId(entityRuleId);
                // if not successful
                if (!specsDeleteResult)
                {
                    response.Message = EntityRulesRelatedMessages.DeleteEntityRuleUnexpectedError;
                    await _unitOfWork.RollbackAsync();
                    return response;
                }
                // Delete entity rule and commit
                await _entityRuleRepository.Delete(entityRuleId);
                await _unitOfWork.CommitAsync();

                // Return success
                response.Result = true;
                response.Success = true;
                response.Message = EntityRulesRelatedMessages.DeleteEntityRuleSuccessful;

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = EntityRulesRelatedMessages.DeleteEntityRuleUnexpectedError;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return response;
        }

        #endregion

        #region Delete Entity Rule Specification

        public async Task<BaseResponse<bool>> DeleteEntityRuleSpecification(string entityRuleId, int specificationId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = "";

            // entity rule identifier is empty
            if (string.IsNullOrEmpty(entityRuleId))
            {
                response.Message = EntityRulesRelatedMessages.EntityRuleIdentifierIsEmpty;
                return response;
            }

            // If entity owner is empty
            else if (specificationId == 0)
            {
                response.Message = EntityRulesRelatedMessages.DeleteEntityRuleSpecIdIsZero;
                return response;
            }

            // Get entity rule specification instance and check if its null
            EntityRuleSpecification entityRuleSpecification = await _entityRuleSpecificationRepository.GetEntityRuleSpecification(entityRuleId, specificationId);
            if (entityRuleSpecification == null)
            {
                response.Message = EntityRulesRelatedMessages.EntityRuleSpecificationNotFound;
                return response;
            }

            bool deleteResult = await _entityRuleSpecificationRepository.DeleteEntityRuleSpecification(entityRuleId, specificationId);
            if (!deleteResult)
            {
                response.Message = EntityRulesRelatedMessages.DeleteEntityRuleSpecUnexpectedError;
                return response;
            }

            response.Result = true;
            response.Success = true;
            response.Message = EntityRulesRelatedMessages.DeleteEntityRuleSpecSuccessful;


            return response;
        }

        #endregion

        #region Get Entity Rule By Id

        /// <summary>
        /// Get entity rule by its identifier
        /// </summary>
        /// <param name="entityRuleId"></param>
        /// <param name="lcode"></param>
        /// <returns></returns>
        public async Task<EntityRuleDTO> GetEntityRuleById(string entityRuleId, string lcode)
        {
            EntityRuleDTO entityRuleDTO = new EntityRuleDTO();

            if (!string.IsNullOrEmpty(entityRuleId) && !string.IsNullOrEmpty(lcode))
            {
                if (lcode.Contains("-"))
                    lcode = lcode.Split('-')[0];

                EntityRule entityRule = await _entityRuleRepository.GetById(entityRuleId);
                if (entityRule != null)
                {
                    IEnumerable<RuleTypeLocalization> ruleTypeLocalizations = await _ruleTypeLocalizationRepository.GetRuleTypesByLocalization(lcode);
                    IEnumerable<BusinessAspectLocalization> businessAspectLocalizations = await _businessAspectLocalizationRepository.GetBusinessAspectsByLocalization(lcode);
                    entityRuleDTO = await HandleEntityRuleData(entityRule, ruleTypeLocalizations, businessAspectLocalizations, lcode);
                }
            }

            return entityRuleDTO;
        }

        #endregion

        #region Get Entity Rules

        /// <summary>
        /// Gets all entity rules of an entity
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="lcode"></param>
        /// <returns></returns>
        public async Task<List<EntityRuleDTO>> GetEntityRules(string entityId, string lcode)
        {
            List<EntityRuleDTO> entityRuleDTOs = new List<EntityRuleDTO>();

            if (!string.IsNullOrEmpty(entityId) && !string.IsNullOrEmpty(lcode))
            {
                IEnumerable<RuleTypeLocalization> ruleTypeLocalizations = await _ruleTypeLocalizationRepository.GetRuleTypesByLocalization(lcode);
                IEnumerable<BusinessAspectLocalization> businessAspectLocalizations = await _businessAspectLocalizationRepository.GetBusinessAspectsByLocalization(lcode);

                IEnumerable<EntityRule> entityRules = await _entityRuleRepository.GetEntityRules(entityId);
                foreach (EntityRule entityRule in entityRules)
                {
                    EntityRuleDTO entityRuleDTO = await HandleEntityRuleData(entityRule, ruleTypeLocalizations, businessAspectLocalizations, lcode);
                    entityRuleDTOs.Add(entityRuleDTO);
                }
            }

            return entityRuleDTOs;
        }

        #endregion

        #region Handle Entity Rule Data

        /// <summary>
        /// Handles the data treatment for the entity rule into a data transfer object instance
        /// </summary>
        /// <param name="entityRule"></param>
        /// <param name="ruleTypeLocalizations"></param>
        /// <param name="businessAspectLocalizations"></param>
        /// <returns></returns>
        private async Task<EntityRuleDTO> HandleEntityRuleData(EntityRule entityRule, IEnumerable<RuleTypeLocalization> ruleTypeLocalizations, IEnumerable<BusinessAspectLocalization> businessAspectLocalizations, string languageCode)
        {
            EntityRuleDTO entityRuleDTO = new EntityRuleDTO();

            // Map it to DTO
            entityRuleDTO = _mapper.Map<EntityRuleDTO>(entityRule);

            // Set rule type display value
            if (ruleTypeLocalizations.Count() != 0)
                entityRuleDTO.RuleTypeDisplayValue = ruleTypeLocalizations.Where(i => i.RuleTypeId.Equals(entityRuleDTO.RuleTypeId)).FirstOrDefault().RuleTypeDisplayValue;

            // Get entity rule specifications
            IEnumerable<EntityRuleSpecification> entityRuleSpecifications = await _entityRuleSpecificationRepository.GetEntityRuleSpecifications(entityRule.EntityRuleId);

            if (entityRule.EntityRuleSpecifications != null)
            {
                foreach (EntityRuleSpecification entityRuleSpec in entityRule.EntityRuleSpecifications)
                {
                    EntityRuleSpecificationDTO entityRuleSpecDTO = _mapper.Map<EntityRuleSpecificationDTO>(entityRuleSpec);

                    entityRuleSpecDTO = await HandleEntityRuleSpecReferences(entityRuleSpecDTO, languageCode);

                    if (entityRuleSpecDTO.BusinessAspectId != 0)
                        entityRuleSpecDTO.BusinessAspectDisplayValue = businessAspectLocalizations.Where(i => i.BusinessAspectId.Equals(entityRuleSpecDTO.BusinessAspectId)).FirstOrDefault().BusinessAspectDisplayValue;
                    else
                        entityRuleSpecDTO.BusinessAspectDisplayValue = "NA";

                    if (entityRuleSpecDTO.BusinessAspectId2 != 0)
                        entityRuleSpecDTO.BusinessAspect2DisplayValue = businessAspectLocalizations.Where(i => i.BusinessAspectId.Equals(entityRuleSpecDTO.BusinessAspectId2)).FirstOrDefault().BusinessAspectDisplayValue;
                    else
                        entityRuleSpecDTO.BusinessAspect2DisplayValue = "NA";

                    entityRuleDTO.EntityRuleSpecificationDTOs.Add(entityRuleSpecDTO);
                }
            }

            return entityRuleDTO;
        }

        #endregion

        #region Get Entity Rule View Model

        public async Task<EntityRuleViewModel> GetEntityRuleViewModel(BaseViewModelRequest entityRuleViewModelRequestDTO)
        {
            EntityRuleViewModel entityRuleViewModel = new EntityRuleViewModel();

            if (entityRuleViewModelRequestDTO != null && !string.IsNullOrEmpty(entityRuleViewModelRequestDTO.WorkerId) && !string.IsNullOrEmpty(entityRuleViewModelRequestDTO.EntityId) && !string.IsNullOrEmpty(entityRuleViewModelRequestDTO.LanguageCode))
            {
                Entity entity = await _entityRepository.GetById(entityRuleViewModelRequestDTO.EntityId);
                EntityWorker entityWorker = await _entityWorkerRepository.GetByWorkerAndEntity(entityRuleViewModelRequestDTO.WorkerId, entityRuleViewModelRequestDTO.EntityId);
                entityRuleViewModel.AllowEdit = entityWorker.IsOwner;

                // If it can change data
                if (entityWorker.IsOwner)
                {
                    // Get All business aspect localized
                    IEnumerable<BusinessAspectLocalization> businessAspectLocalizations = await _businessAspectLocalizationRepository.GetBusinessAspectsByLocalization(entityRuleViewModelRequestDTO.LanguageCode);

                    // Map it to a transferable object
                    foreach (BusinessAspectLocalization businessAspectLocalization in businessAspectLocalizations)
                        entityRuleViewModel.BusinessAspectsLocalizeds.Add(_mapper.Map<BusinessAspectLocalizedDTO>(businessAspectLocalization));

                    // Get All rule type localized
                    IEnumerable<RuleTypeLocalization> ruleTypeLocalizations = await _ruleTypeLocalizationRepository.GetRuleTypesByLocalization(entityRuleViewModelRequestDTO.LanguageCode);
                    foreach (RuleTypeLocalization ruleTypeLocalization in ruleTypeLocalizations)
                    {
                        // Map it to transferable object
                        RuleTypeLocalizedDTO ruleTypeLocalizedDTO = new RuleTypeLocalizedDTO();
                        ruleTypeLocalizedDTO = _mapper.Map<RuleTypeLocalizedDTO>(ruleTypeLocalization);
                        RuleType ruleType = await _ruleTypeRepository.GetById(ruleTypeLocalizedDTO.RuleTypeId);
                        ruleTypeLocalizedDTO.MultipleSpecification = ruleType.MultipleSpecification;

                        // Get relations between this rule type and its business aspect
                        List<RuleTypeBusinessAspect> ruleTypeBusinessAspects = await _ruleTypeBusinessAspectRepository.GetRuleTypeBusinessAspectsByRuleTypeId(ruleTypeLocalization.RuleTypeId);

                        // For each relation
                        foreach (RuleTypeBusinessAspect ruleTypeBusinessAspect in ruleTypeBusinessAspects)
                        {
                            // Get Business aspect localized record and add it to the rule type
                            BusinessAspectLocalizedDTO businessAspectLocalizedDTO = entityRuleViewModel.BusinessAspectsLocalizeds.Where(i => i.BusinessAspectId == ruleTypeBusinessAspect.BusinessAspectId).FirstOrDefault();
                            if (businessAspectLocalizedDTO != null)
                                ruleTypeLocalizedDTO.BusinessAspectLocalizedDTOs.Add(businessAspectLocalizedDTO);

                        }

                        // Add Rule types
                        entityRuleViewModel.RuleTypeLocalizeds.Add(ruleTypeLocalizedDTO);
                    }

                    IEnumerable<EntityRule> entityRules = await _entityRuleRepository.GetEntityRules(entityRuleViewModelRequestDTO.EntityId);
                    foreach (EntityRule entityRule in entityRules)
                    {
                        EntityRuleDTO entityRuleDTO = await HandleEntityRuleData(entityRule, ruleTypeLocalizations, businessAspectLocalizations, entityRuleViewModelRequestDTO.LanguageCode);
                        entityRuleViewModel.EntityRules.Add(entityRuleDTO);
                    }
                }
            }

            return entityRuleViewModel;
        }

        #endregion

        #region Update Entity Rule

        public async Task<BaseResponse<bool>> UpdateEntityRule(EntityRuleDTO entityRule)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = EntityRulesRelatedMessages.UpdateEntityRuleUnexpectedError;

            if (entityRule != null)
            {
                EntityRule instance = await _entityRuleRepository.GetById(entityRule.EntityRuleId);
                if (instance == null)
                {
                    response.Message = EntityRulesRelatedMessages.EntityRuleNotFound;
                    return response;
                }

                else if (entityRule.RuleTypeId == 0)
                {
                    response.Message = EntityRulesRelatedMessages.AddNewEntityRuleTypeIsZero;
                    return response;
                }

                else if (string.IsNullOrEmpty(entityRule.EntityId))
                {
                    response.Message = EntityRulesRelatedMessages.AddNewEntityEntityIdEmpty;
                    return response;
                }

                else if (entityRule.EntityRuleSpecificationDTOs.Count() == 0)
                {
                    response.Message = EntityRulesRelatedMessages.AddNewEntityNoSpecificationsFound;
                    return response;
                }

                // Get entity and check ifs null
                Entity destinationEntity = await _entityRepository.GetById(entityRule.EntityId);
                if (destinationEntity == null)
                {
                    response.Message = EntityRulesRelatedMessages.AddNewEntityRuleEntityNotFound;
                    return response;
                }
                else
                {
                    _mapper.Map(entityRule, instance);
                    await _entityRuleRepository.Update(instance);
                    response.Success = true;
                    response.Message = EntityRulesRelatedMessages.UpdateEntityRuleSuccessful;
                }
            }

            return response;
        }

        #endregion

        #region Update Entity Rule Specification

        public async Task<BaseResponse<bool>> UpdateEntityRuleSpecification(EntityRuleSpecificationDTO entityRuleSpecification)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = EntityRulesRelatedMessages.UpdateEntityRuleSpecUnexpectedError;

            if (entityRuleSpecification != null)
            {
                EntityRuleSpecification instance = await _entityRuleSpecificationRepository.GetEntityRuleSpecification(entityRuleSpecification.EntityRuleId, entityRuleSpecification.SpecificationId);
                if (instance == null)
                {
                    response.Message = EntityRulesRelatedMessages.EntityRuleSpecificationNotFound;
                    return response;
                }

                // If the entity rule identifier is empty, send error
                else if (string.IsNullOrEmpty(entityRuleSpecification.EntityRuleId))
                {
                    response.Message = EntityRulesRelatedMessages.AddEntityRuleSpecEntityRuleIsEmpty;
                    return response;
                }

                _mapper.Map(entityRuleSpecification, instance);
                await _entityRuleSpecificationRepository.UpdateEntityRuleSpecification(instance);
                response.Success = true;
                response.Message = EntityRulesRelatedMessages.UpdateEntityRuleSpecSuccessful;
            }

            return response;
        }

        #endregion

        #region Delete Entity Rule Specifications

        public async Task<BaseResponse<bool>> DeleteEntityRuleSpecifications(string entityRuleId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Message = EntityRulesRelatedMessages.DeleteEntityRuleSpecUnexpectedError;

            if (string.IsNullOrEmpty(entityRuleId))
            {
                response.Message = EntityRulesRelatedMessages.EntityRuleIdentifierIsEmpty;
                return response;
            }

            else
            {
                await _entityRuleSpecificationRepository.DeleteEntityRuleSpecificationsByRuleId(entityRuleId);
                response.Success = true;
                response.Message = EntityRulesRelatedMessages.DeleteEntityRuleSpecSuccessful;
            }

            return response;
        }

        #endregion

        #endregion
    }
}
