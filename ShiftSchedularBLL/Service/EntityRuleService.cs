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
        private readonly ISkillService _skillService;
        private readonly IShiftService _shiftService;
        private readonly ILanguageAccessor _languageAccessor;

        #region Constructor

        public EntityRuleService(
            IMapper mapper,
            IGeneralService generalService,
            IUnitOfWork unitOfWork,
            // IGenericRepository<Entity> entityRepository,
            ISkillService skillService,
            //IEntityRuleRepository entityRuleRepository,
            //IEntityWorkerRepository entityWorkerRepository,
            //IEntityRuleSpecificationRepository entityRuleSpecificationRepository,
            //IGenericRepository<RuleType> ruleTypeRepository,
            //IRuleTypeLocalizationRepository ruleTypeLocalizationRepository,
            //IGenericRepository<BusinessAspect> businessAspectRepository,
            //IBusinessAspectLocalizationRepository businessAspectLocalizationRepository,
            //IRuleTypeBusinessAspectRepository ruleTypeBusinessAspectRepository,
            IShiftService shiftService,
            ILanguageAccessor languageAccessor)
        {
            _mapper = mapper;
            _generalService = generalService;
            _unitOfWork = unitOfWork;
            // _entityRepository = entityRepository;
            _skillService = skillService;
            //_entityRuleRepository = entityRuleRepository;
            //_entityWorkerRepository = entityWorkerRepository;
            //_entityRuleSpecificationRepository = entityRuleSpecificationRepository;
            //_ruleTypeRepository = ruleTypeRepository;
            //_bussinessAspectRepository = businessAspectRepository;
            //_ruleTypeLocalizationRepository = ruleTypeLocalizationRepository;
            //_businessAspectLocalizationRepository = businessAspectLocalizationRepository;
            //_ruleTypeBusinessAspectRepository = ruleTypeBusinessAspectRepository;
            _shiftService = shiftService;
            _languageAccessor = languageAccessor;
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

                else if (addEntityRuleDTO.EntityId == Guid.Empty) { 
                    response.Message = EntityRulesRelatedMessages.AddNewEntityEntityIdEmpty;
                    return response;
                }

                else if (addEntityRuleDTO.EntityRuleSpecifications.Count() == 0)
                {
                    response.Message = EntityRulesRelatedMessages.AddNewEntityNoSpecificationsFound;
                    return response;
                }

                // Get entity and check ifs null
                Entity destinationEntity = await _unitOfWork.GetGenericRepository<Entity>().GetById(addEntityRuleDTO.EntityId);
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
                        newRule.EntityRuleId = new Guid();

                        // Clear the list due to mapping
                        if (newRule.EntityRuleSpecifications.Count != 0)
                            newRule.EntityRuleSpecifications = new List<EntityRuleSpecification>();

                        await _unitOfWork.EntityRuleRepository.Add(newRule);

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
                if (addEntityRuleSpecificationDTO.EntityRuleId == Guid.Empty)
                {
                    response.Message = EntityRulesRelatedMessages.AddEntityRuleSpecEntityRuleIsEmpty;
                    return response;
                }

                // Map rule specification and add it to the database
                EntityRuleSpecification ruleSpecification = _mapper.Map<EntityRuleSpecification>(addEntityRuleSpecificationDTO);
                ruleSpecification = await _unitOfWork.EntityRuleSpecificationRepository.AddEntityRuleSpecification(ruleSpecification);

                EntityRuleSpecificationDTO entityRuleSpecification = _mapper.Map<EntityRuleSpecificationDTO>(ruleSpecification);
                entityRuleSpecification = await HandleEntityRuleSpecReferences(entityRuleSpecification, _languageAccessor.GetLanguageCode());

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
                BusinessAspect businessAspect = await _unitOfWork.GetGenericRepository<BusinessAspect>().GetById(entityRuleSpecificationDTO.BusinessAspectId);
                if (businessAspect != null && businessAspect.BusinessAspectName.Equals(BusinessAspectsConstants.SHIFTS))
                {
                    ShiftDTO shiftDTO = await _shiftService.GetShiftById(_generalService.ParseStringToGuid(entityRuleSpecificationDTO.AspectReferenceId));
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
                BusinessAspect businessAspect = await _unitOfWork.GetGenericRepository<BusinessAspect>().GetById(entityRuleSpecificationDTO.BusinessAspectId2);
                if (businessAspect != null && businessAspect.BusinessAspectName.Equals(BusinessAspectsConstants.SHIFTS))
                {
                    ShiftDTO shiftDTO = await _shiftService.GetShiftById(_generalService.ParseStringToGuid(entityRuleSpecificationDTO.AspectReferenceId2));
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
        public async Task<BaseResponse<bool>> DeleteEntityRule(Guid entityId, Guid entityRuleId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = "";

            // If entity owner is empty
            if (entityId == Guid.Empty)
            {
                response.Message = EntityRulesRelatedMessages.EntityRuleEntityIdentifierIsEmpty;
                return response;
            }

            // entity rule identifier is empty
            else if (entityRuleId == Guid.Empty)
            {
                response.Message = EntityRulesRelatedMessages.EntityRuleIdentifierIsEmpty;
                return response;
            }

            // Get entity rule instance from the database
            EntityRule entityRule = await _unitOfWork.EntityRuleRepository.GetById(entityRuleId);
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
                bool specsDeleteResult = await _unitOfWork.EntityRuleSpecificationRepository.DeleteEntityRuleSpecificationsByRuleId(entityRuleId);
                // if not successful
                if (!specsDeleteResult)
                {
                    response.Message = EntityRulesRelatedMessages.DeleteEntityRuleUnexpectedError;
                    await _unitOfWork.RollbackAsync();
                    return response;
                }
                // Delete entity rule and commit
                await _unitOfWork.EntityRuleRepository.Delete(entityRuleId);
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

        public async Task<BaseResponse<bool>> DeleteEntityRuleSpecification(Guid entityRuleId, int specificationId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = "";

            // entity rule identifier is empty
            if (entityRuleId == Guid.Empty)
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
            EntityRuleSpecification entityRuleSpecification = await _unitOfWork.EntityRuleSpecificationRepository.GetEntityRuleSpecification(entityRuleId, specificationId);
            if (entityRuleSpecification == null)
            {
                response.Message = EntityRulesRelatedMessages.EntityRuleSpecificationNotFound;
                return response;
            }

            bool deleteResult = await _unitOfWork.EntityRuleSpecificationRepository.DeleteEntityRuleSpecification(entityRuleId, specificationId);
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
        public async Task<EntityRuleDTO> GetEntityRuleById(Guid entityRuleId, string lcode)
        {
            EntityRuleDTO entityRuleDTO = new EntityRuleDTO();

            if (entityRuleId != Guid.Empty && !string.IsNullOrEmpty(lcode))
            {
                if (lcode.Contains("-"))
                    lcode = lcode.Split('-')[0];

                EntityRule entityRule = await _unitOfWork.EntityRuleRepository.GetById(entityRuleId);
                if (entityRule != null)
                {
                    IEnumerable<RuleTypeLocalization> ruleTypeLocalizations = await _unitOfWork.RuleTypeLocalizationRepository.GetRuleTypesByLocalization(lcode);
                    IEnumerable<BusinessAspectLocalization> businessAspectLocalizations = await _unitOfWork.BusinessAspectLocalizationRepository.GetBusinessAspectsByLocalization(lcode);
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
        public async Task<List<EntityRuleDTO>> GetEntityRules(Guid entityId, string lcode)
        {
            List<EntityRuleDTO> entityRuleDTOs = new List<EntityRuleDTO>();

            if (entityId != Guid.Empty && !string.IsNullOrEmpty(lcode))
            {
                IEnumerable<RuleTypeLocalization> ruleTypeLocalizations = await _unitOfWork.RuleTypeLocalizationRepository.GetRuleTypesByLocalization(lcode);
                IEnumerable<BusinessAspectLocalization> businessAspectLocalizations = await _unitOfWork.BusinessAspectLocalizationRepository.GetBusinessAspectsByLocalization(lcode);

                IEnumerable<EntityRule> entityRules = await _unitOfWork.EntityRuleRepository.GetEntityRules(entityId);
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
            IEnumerable<EntityRuleSpecification> entityRuleSpecifications = await _unitOfWork.EntityRuleSpecificationRepository.GetEntityRuleSpecifications(entityRule.EntityRuleId);

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

            if (entityRuleViewModelRequestDTO != null && !string.IsNullOrEmpty(entityRuleViewModelRequestDTO.WorkerId) && entityRuleViewModelRequestDTO.EntityId != Guid.Empty)
            {
                Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(entityRuleViewModelRequestDTO.EntityId);
                EntityWorker entityWorkerInstance = await _unitOfWork.EntityWorkerRepository.GetByWorkerAndEntity(entityRuleViewModelRequestDTO.WorkerId, entityRuleViewModelRequestDTO.EntityId);
                entityRuleViewModel.AllowEdit = await _unitOfWork.EntityPermissionRepository.CanUserEditEntity(entityRuleViewModelRequestDTO.EntityId, entityRuleViewModelRequestDTO.WorkerId);
                entityRuleViewModel.ParentEntityId = entity?.ParentEntityId;

                // If it can change data
                if (entityRuleViewModel.AllowEdit) // entityWorkerInstance.IsOwner
                {
                    // Get All business aspect localized
                    IEnumerable<BusinessAspectLocalization> businessAspectLocalizations = await _unitOfWork.BusinessAspectLocalizationRepository.GetBusinessAspectsByLocalization(_languageAccessor.GetLanguageCode());

                    // Map it to a transferable object
                    foreach (BusinessAspectLocalization businessAspectLocalization in businessAspectLocalizations)
                        entityRuleViewModel.BusinessAspectsLocalizeds.Add(_mapper.Map<BusinessAspectLocalizedDTO>(businessAspectLocalization));

                    // Get All rule type localized
                    IEnumerable<RuleTypeLocalization> ruleTypeLocalizations = await _unitOfWork.RuleTypeLocalizationRepository.GetRuleTypesByLocalization(_languageAccessor.GetLanguageCode());
                    foreach (RuleTypeLocalization ruleTypeLocalization in ruleTypeLocalizations)
                    {
                        // Map it to transferable object
                        RuleTypeLocalizedDTO ruleTypeLocalizedDTO = new RuleTypeLocalizedDTO();
                        ruleTypeLocalizedDTO = _mapper.Map<RuleTypeLocalizedDTO>(ruleTypeLocalization);
                        RuleType ruleType = await _unitOfWork.GetGenericRepository<RuleType>().GetById(ruleTypeLocalizedDTO.RuleTypeId);
                        ruleTypeLocalizedDTO.MultipleSpecification = ruleType.MultipleSpecification;
                        ruleTypeLocalizedDTO.IsSpecValuesBoolean = ruleType.IsSpecValuesBoolean;

                        // Get relations between this rule type and its business aspect
                        List<RuleTypeBusinessAspect> ruleTypeBusinessAspects = await _unitOfWork.RuleTypeBusinessAspectRepository.GetRuleTypeBusinessAspectsByRuleTypeId(ruleTypeLocalization.RuleTypeId);

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

                    IEnumerable<EntityRule> entityRules = await _unitOfWork.EntityRuleRepository.GetEntityRules(entityRuleViewModelRequestDTO.EntityId);
                    foreach (EntityRule entityRule in entityRules)
                    {
                        EntityRuleDTO entityRuleDTO = await HandleEntityRuleData(entityRule, ruleTypeLocalizations, businessAspectLocalizations, _languageAccessor.GetLanguageCode());
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
                EntityRule instance = await _unitOfWork.EntityRuleRepository.GetById(entityRule.EntityRuleId);
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

                else if (entityRule.EntityId == Guid.Empty)
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
                Entity destinationEntity = await _unitOfWork.GetGenericRepository<Entity>().GetById(entityRule.EntityId);
                if (destinationEntity == null)
                {
                    response.Message = EntityRulesRelatedMessages.AddNewEntityRuleEntityNotFound;
                    return response;
                }
                else
                {
                    _mapper.Map(entityRule, instance);
                    await _unitOfWork.EntityRuleRepository.Update(instance);
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
                EntityRuleSpecification instance = await _unitOfWork.EntityRuleSpecificationRepository.GetEntityRuleSpecification(entityRuleSpecification.EntityRuleId, entityRuleSpecification.SpecificationId);
                if (instance == null)
                {
                    response.Message = EntityRulesRelatedMessages.EntityRuleSpecificationNotFound;
                    return response;
                }

                // If the entity rule identifier is empty, send error
                else if (entityRuleSpecification.EntityRuleId == Guid.Empty)
                {
                    response.Message = EntityRulesRelatedMessages.AddEntityRuleSpecEntityRuleIsEmpty;
                    return response;
                }

                _mapper.Map(entityRuleSpecification, instance);
                await _unitOfWork.EntityRuleSpecificationRepository.UpdateEntityRuleSpecification(instance);
                response.Success = true;
                response.Message = EntityRulesRelatedMessages.UpdateEntityRuleSpecSuccessful;
            }

            return response;
        }

        #endregion

        #region Delete Entity Rule Specifications

        public async Task<BaseResponse<bool>> DeleteEntityRuleSpecifications(Guid entityRuleId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Message = EntityRulesRelatedMessages.DeleteEntityRuleSpecUnexpectedError;

            if (entityRuleId == Guid.Empty)
            {
                response.Message = EntityRulesRelatedMessages.EntityRuleIdentifierIsEmpty;
                return response;
            }

            else
            {
                await _unitOfWork.EntityRuleSpecificationRepository.DeleteEntityRuleSpecificationsByRuleId(entityRuleId);
                response.Success = true;
                response.Message = EntityRulesRelatedMessages.DeleteEntityRuleSpecSuccessful;
            }

            return response;
        }

        #endregion

        #region Get Specific Rules

        public async Task<List<EntityRuleDTO>> GetSpecificRules(Guid entityId, List<string> filteredRules, string languageCode)
        {
            if(filteredRules.Count != 0)
            {
                List<EntityRuleDTO> entityRules = await this.GetEntityRules(entityId, languageCode);

                if (entityRules != null && entityRules.Count != 0)
                {
                    List<EntityRuleDTO> filteredEntityRules = entityRules.Where(i => filteredRules.Contains(i.EntityRuleId.ToString())).ToList();
                    return filteredEntityRules;
                }
            }

            return new List<EntityRuleDTO>();
            
        }

        #endregion

        #region Get Total Entity Rules

        public async Task<int> GetTotalEntityRules(Guid entityId)
        {
            int count = 0;

            if (entityId != Guid.Empty)
            {
                IEnumerable<EntityRule> entityRules = await _unitOfWork.EntityRuleRepository.GetEntityRules(entityId);
                count = entityRules.Count();
            }

            return count;
        }

        #endregion

        #endregion
    }
}
