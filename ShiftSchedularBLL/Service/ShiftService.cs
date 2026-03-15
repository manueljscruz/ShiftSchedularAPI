using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.Shared;
using ShiftSchedularRL.Resources.ShiftManagement;

namespace ShiftSchedularBLL.Service
{
    public class ShiftService : IShiftService
    {
        private readonly IMapper _mapper;
        private readonly IGeneralService _generalService;
        private readonly IShiftTemplateService _shiftTemplateService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILanguageAccessor _languageAccessor;

        #region Constructor

        public ShiftService(IMapper mapper,
            IGeneralService generalService,
            IShiftTemplateService shiftTemplateService,
            IUnitOfWork unitOfWork,
            ILanguageAccessor languageAccessor)
        {
            _mapper = mapper;
            _generalService = generalService;
            _shiftTemplateService = shiftTemplateService;
            _unitOfWork = unitOfWork;
            _languageAccessor = languageAccessor;
        }

        #endregion

        #region Methods

        #region Add Entity Shift

        /// <summary>
        /// Adds an entity Shift
        /// If entity shift breaks are included, those are added as well
        /// </summary>
        /// <param name="addShiftDTO"></param>
        /// <returns></returns>
        public async Task<BaseResponse<ShiftDTO>> AddEntityShift(AddShiftDTO addShiftDTO)
        {
            BaseResponse<ShiftDTO> response = new BaseResponse<ShiftDTO>();
            response.Success = false;
            response.Message = ShiftRelatedMessages.AddNewShiftUnexpectedError;

            if (addShiftDTO != null)
            {
                // No destination entity
                if (addShiftDTO.EntityId == Guid.Empty)
                {
                    response.Message = ShiftRelatedMessages.ShiftEntityIdIsNull;
                    return response;
                }

                // Name is Empty
                else if (string.IsNullOrEmpty(addShiftDTO.ShiftName))
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftNameEmpty;
                    return response;
                }

                // No shift duration
                else if (addShiftDTO.ShiftDuration == TimeSpan.Zero)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftDurationIsNull;
                    return response;
                }

                // Get entity and check ifs null
                Entity destinationEntity = await _unitOfWork.GetGenericRepository<Entity>().GetById(addShiftDTO.EntityId);
                if (destinationEntity == null)
                {
                    response.Message = ShiftRelatedMessages.EntityNotFound;
                    return response;
                }
                else
                {
                    try
                    {
                        await _unitOfWork.BeginTransactionAsync();

                        // Map shift, create Id and add it
                        Shift newShift = _mapper.Map<Shift>(addShiftDTO);
                        newShift.ShiftId = new Guid();
                        newShift = await _unitOfWork.ShiftRepository.Add(newShift);

                        ShiftDTO shiftDTO = _mapper.Map<ShiftDTO>(newShift);


                        // If there are any shift breaks, add them
                        if (addShiftDTO.ShiftBreakDTOs.Count != 0)
                        {
                            foreach (AddShiftBreakDTO addShiftBreakDTO in addShiftDTO.ShiftBreakDTOs)
                            {
                                addShiftBreakDTO.ShiftId = newShift.ShiftId;
                                BaseResponse<ShiftBreakDTO> shiftBreakDTOResponse = await this.AddEntityShiftBreak(addShiftBreakDTO);

                                // If shift break was not added successfuly
                                if (!shiftBreakDTOResponse.Success)
                                {
                                    response.Message = shiftBreakDTOResponse.Message;
                                    await _unitOfWork.RollbackAsync();
                                    return response;
                                }
                                else
                                {
                                    shiftDTO.ShiftBreakDTOs.Add(shiftBreakDTOResponse.Result);
                                }
                            }
                        }

                        // Save changes in the database and assign values to the response result
                        await _unitOfWork.CommitAsync();
                        response.Success = true;
                        response.Message = ShiftRelatedMessages.AddNewShiftSuccessful;

                        response.Result = await GetShiftById(newShift.ShiftId);
                    }
                    catch (Exception ex)
                    {
                        await _unitOfWork.RollbackAsync();
                        response.Message = ShiftRelatedMessages.AddNewShiftUnexpectedError;
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

        #region Add Entity Shift Break

        /// <summary>
        /// Adds a new shift break 
        /// </summary>
        /// <param name="addShiftBreakDTO"></param>
        /// <returns></returns>
        public async Task<BaseResponse<ShiftBreakDTO>> AddEntityShiftBreak(AddShiftBreakDTO addShiftBreakDTO)
        {
            BaseResponse<ShiftBreakDTO> response = new BaseResponse<ShiftBreakDTO>();
            response.Success = false;
            response.Message = ShiftRelatedMessages.AddNewShiftBreakUnexpectedError;

            if (addShiftBreakDTO != null)
            {
                if (addShiftBreakDTO.ShiftId == Guid.Empty)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftBreakEmptyShift;
                    return response;
                }

                else if (addShiftBreakDTO.ShiftBreakTypeId == 0)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftBreakEmptyShift;
                    return response;
                }

                ShiftBreakType shiftBreakType = await _unitOfWork.GetGenericRepository<ShiftBreakType>().GetById(addShiftBreakDTO.ShiftBreakTypeId);

                if (shiftBreakType == null)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftBreakBreakTypeNotFound;
                    return response;
                }

                else if (addShiftBreakDTO.ShiftBreakDuration == TimeSpan.Zero)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftBreakDurationIsNull;
                    return response;
                }

                ShiftBreak shiftBreak = _mapper.Map<ShiftBreak>(addShiftBreakDTO);
                shiftBreak.ShiftBreakId = new Guid();

                shiftBreak = await _unitOfWork.ShiftBreakRepository.Add(shiftBreak);
                response.Success = true;
                response.Result = _mapper.Map<ShiftBreakDTO>(shiftBreak);
                response.Message = ShiftRelatedMessages.AddNewShiftBreakSuccessful;
            }

            return response;
        }

        #endregion

        #region Delete Entity Shift

        /// <summary>
        /// Deletes a shift related to this entity
        /// If any shift breaks exists, they are also deleted
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="shiftId"></param>
        /// <returns></returns>
        public async Task<BaseResponse<bool>> DeleteEntityShift(Guid entityId, Guid shiftId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = SharedMessages.UnexpectedError;

            if (entityId == Guid.Empty)
            {
                response.Message = ShiftRelatedMessages.ShiftEntityIdIsNull;
                return response;
            }
            else if (shiftId == Guid.Empty)
            {
                response.Message = ShiftRelatedMessages.ShiftIdIsNull;
                return response;
            }

            Shift shiftInstance = await _unitOfWork.ShiftRepository.GetById(shiftId);
            if (shiftInstance == null)
            {
                response.Message = ShiftRelatedMessages.ShiftNotFound;
                return response;
            }

            IEnumerable<ShiftBreak> shiftBreaks = await _unitOfWork.ShiftBreakRepository.GetBreaksByShiftId(shiftId);
            if (shiftBreaks == null)
            {
                response.Message = ShiftRelatedMessages.DeleteShiftBreaksNotFound;
                return response;
            }

            // Get related schedule entries
            int count = await _unitOfWork.EntityScheduleRepository.GetShiftForwardEntriesCount(entityId, shiftId, DateTime.UtcNow);

            // Error occurred
            if (count == -1)
            {
                return response;
            }

            // Existing scheduling shift entries
            else if (count > 0)
            {
                response.Message = ShiftRelatedMessages.DeleteShiftExistingScheduleEntries;
                return response;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // If there are shift breaks related, remove them
                if (shiftBreaks.Count() != 0)
                    await _unitOfWork.ShiftBreakRepository.DeleteRange(shiftBreaks);

                EntityShiftRotation entityShiftRotation = await _unitOfWork.EntityShiftRotationRepository.GetEntityShiftRotation(entityId, shiftId);
                if (entityShiftRotation != null)
                    await _unitOfWork.EntityShiftRotationRepository.DeleteEntityShiftRotation(entityShiftRotation);

                // Get related rules
                List<EntityRule> shiftRelatedRules = await _unitOfWork.EntityRuleRepository.GetEntityRulesRelatedToShifts(entityId);
                if (shiftRelatedRules.Count != 0)
                {
                    // For each rule
                    foreach (EntityRule rule in shiftRelatedRules)
                    {
                        // Get related specifications where the shift is referenced
                        IEnumerable<EntityRuleSpecification> entityRuleSpecifications = rule.EntityRuleSpecifications.Where(i => i.AspectReferenceId.Equals(shiftId.ToString()) || i.AspectReferenceId2.Equals(shiftId.ToString()));
                        bool result = await _unitOfWork.EntityRuleSpecificationRepository.DeleteRange(entityRuleSpecifications);

                        // If minus these references, the rule as no more specifications, remove rule all together
                        if (rule.EntityRuleSpecifications.Count - entityRuleSpecifications.Count() == 0)
                        {
                            await _unitOfWork.EntityRuleRepository.Delete(rule.EntityRuleId);
                        }
                    }
                }

                // Delete previous entries
                bool scheduleEntriesOp = await _unitOfWork.EntityScheduleRepository.DeletePreviousShiftEntries(entityId, shiftId, DateTime.UtcNow);


                // Get Specific Worker & Bot Shift Assignments
                IEnumerable<EntityUserBotShiftAssigned> entityUserBotShiftAssigneds = await _unitOfWork.EntityUserBotShiftAssignedsRepository.GetByEntityIdAndShiftId(entityId, shiftId);
                IEnumerable<EntityWorkerShiftAssigned> entityWorkerShiftAssigneds = await _unitOfWork.EntityWorkerShiftAssignedsRepository.GetByEntityIdAndShiftId(entityId, shiftId);

                if (entityUserBotShiftAssigneds.Count() != 0)
                    await _unitOfWork.EntityUserBotShiftAssignedsRepository.DeleteRange(entityUserBotShiftAssigneds);

                if (entityWorkerShiftAssigneds.Count() != 0)
                    await _unitOfWork.EntityWorkerShiftAssignedsRepository.DeleteRange(entityWorkerShiftAssigneds);

                await _unitOfWork.ShiftRepository.Delete(shiftInstance.ShiftId);

                await _unitOfWork.CommitAsync();
                response.Success = true;
                response.Message = ShiftRelatedMessages.ShiftRemovedSuccessfuly;
                response.Result = true;

            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = ShiftRelatedMessages.DeleteShiftUnexpectedError;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return response;
        }

        #endregion

        #region Delete Entity Shift Break

        /// <summary>
        /// Deletes a shift break associated to a shift of a particular entity
        /// </summary>
        /// <param name="deleteEntityShiftBreak"></param>
        /// <returns></returns>
        public async Task<BaseResponse<bool>> DeleteEntityShiftBreak(Guid shiftBreakId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = ShiftRelatedMessages.DeleteShiftBreakUnexpectedError;

            if (shiftBreakId == Guid.Empty)
            {
                response.Message = ShiftRelatedMessages.DeleteShiftBreakIdIsNull;
                return response;
            }

            ShiftBreak shiftBreak = await _unitOfWork.ShiftBreakRepository.GetById(shiftBreakId);
            if (shiftBreak == null)
            {
                response.Message = ShiftRelatedMessages.DeleteShiftBreaksNotFound;
                return response;
            }

            await _unitOfWork.ShiftBreakRepository.Delete(shiftBreak.ShiftBreakId);

            response.Success = true;
            response.Message = ShiftRelatedMessages.DeleteShiftBreakSuccessful;



            return response;
        }

        #endregion

        #region Get Entity Shifts View Model

        /// <summary>
        /// Gets all the shift data related to this entity
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="lcode"></param>
        /// <returns></returns>
        public async Task<ShiftViewModel> GetEntityShiftsViewModel(BaseViewModelRequest shiftViewModelRequestDTO)
        {
            ShiftViewModel shiftViewModel = new ShiftViewModel();

            // If necessary data is different than empty
            if (shiftViewModelRequestDTO != null && shiftViewModelRequestDTO.EntityId != Guid.Empty && !string.IsNullOrEmpty(shiftViewModelRequestDTO.WorkerId))
            {
                Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(shiftViewModelRequestDTO.EntityId);
                EntityWorker entityWorkerInstance = await _unitOfWork.EntityWorkerRepository.GetByWorkerAndEntity(shiftViewModelRequestDTO.WorkerId, shiftViewModelRequestDTO.EntityId);
                shiftViewModel.AllowEdit = entityWorkerInstance.IsOwner;

                // If it can change data
                if (entityWorkerInstance.IsOwner)
                {
                    // Get Shift Break Types Localized
                    IEnumerable<ShiftBreakTypeLocalization> shiftBreakTypeLocalizations = await _unitOfWork.ShiftBreakTypeLocalizationRepository.GetShiftBreaksTypeLocalized(_languageAccessor.GetLanguageCode());
                    foreach (ShiftBreakTypeLocalization shiftBreakTypeLocalization in shiftBreakTypeLocalizations)
                        shiftViewModel.ShiftBreakTypeLocalizeds.Add(_mapper.Map<ShiftBreakTypeLocalization, ShiftBreakTypeLocalizedDTO>(shiftBreakTypeLocalization));

                    // Get Shift Breaks Templates
                    shiftViewModel.ShiftBreakTemplates = await _shiftTemplateService.GetShiftBreakTemplates(_languageAccessor.GetLanguageCode());

                    shiftViewModel.ShiftTemplates = await _shiftTemplateService.GetShiftTemplates(_languageAccessor.GetLanguageCode());

                    // Get Shifts
                    IEnumerable<Shift> shifts = await _unitOfWork.ShiftRepository.GetEntityShifts(shiftViewModelRequestDTO.EntityId);
                    foreach (Shift shift in shifts)
                    {
                        ShiftDTO shiftDTO = await HandleShiftData(shift, shiftBreakTypeLocalizations);
                        shiftViewModel.Shifts.Add(shiftDTO);
                    }

                    shiftViewModel.ShiftRotations = await this.GetEntityShiftRotations(shiftViewModelRequestDTO.EntityId);
                }
            }

            return shiftViewModel;
        }

        #endregion

        #region Get Shift By Id

        /// <summary>
        /// Gets shift by its identifier
        /// </summary>
        /// <param name="shiftId"></param>
        /// <returns></returns>
        public async Task<ShiftDTO> GetShiftById(Guid shiftId)
        {
            ShiftDTO shiftDTO = new ShiftDTO();

            // If there is a shift id
            if (shiftId != Guid.Empty)
            {
                // Get shift and proceed if its different than null
                Shift shift = await _unitOfWork.ShiftRepository.GetById(shiftId);
                if (shift != null)
                {
                    IEnumerable<ShiftBreakTypeLocalization> shiftBreakTypeLocalizations = await _unitOfWork.ShiftBreakTypeLocalizationRepository.GetShiftBreaksTypeLocalized(_languageAccessor.GetLanguageCode());
                    shiftDTO = await HandleShiftData(shift, shiftBreakTypeLocalizations);
                }
            }

            return shiftDTO;
        }

        #endregion

        #region Get Entity Shifts

        /// <summary>
        /// Get Entity Shifts
        /// </summary>
        /// <param name="entityId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ShiftDTO>> GetEntityShifts(Guid entityId)
        {
            List<ShiftDTO> shiftsDTO = new List<ShiftDTO>();

            if (entityId != Guid.Empty)
            {
                Entity entity = await _unitOfWork.GetGenericRepository<Entity>().GetById(entityId);
                if (entity != null)
                {
                    IEnumerable<Shift> shifts = await _unitOfWork.ShiftRepository.GetEntityShifts(entityId);
                    foreach (Shift shift in shifts)
                    {
                        ShiftDTO shiftDTO = _mapper.Map<ShiftDTO>(shift);
                        shiftsDTO.Add(shiftDTO);
                    }

                }
            }
            return shiftsDTO;
        }

        #endregion

        #region Handle Shift Data 

        /// <summary>
        /// Takes the shift instance into a data transfer object instance based on the localization
        /// </summary>
        /// <param name="shift"></param>
        /// <param name="lcode"></param>
        /// <returns></returns>
        private async Task<ShiftDTO> HandleShiftData(Shift shift, IEnumerable<ShiftBreakTypeLocalization> shiftBreakTypeLocalizations)
        {
            ShiftDTO shiftDTO = new ShiftDTO();

            // Map it to DTO object
            shiftDTO = _mapper.Map<ShiftDTO>(shift);

            // Use navigation property instead of querying - ShiftBreaks already loaded via Include
            IEnumerable<ShiftBreak> shiftBreaks = shift.ShiftBreaks ?? Enumerable.Empty<ShiftBreak>();

            // If any, map them to the 
            if (shiftBreaks.Count() != 0 && shiftBreakTypeLocalizations.Count() != 0)
            {
                // For each shift break, map it, set shift break type localized value and add to the list
                foreach (ShiftBreak shiftBreak in shiftBreaks)
                {
                    ShiftBreakDTO shiftBreakDTO = _mapper.Map<ShiftBreakDTO>(shiftBreak);
                    ShiftBreakTypeLocalization shiftBreakTypeLocalization = shiftBreakTypeLocalizations.Where(i => i.ShiftBreakTypeId.Equals(shiftBreak.ShiftBreakTypeId)).FirstOrDefault();
                    shiftBreakDTO.ShiftBreakTypeDisplay = shiftBreakTypeLocalizations.Where(i => i.ShiftBreakTypeId.Equals(shiftBreak.ShiftBreakTypeId)).FirstOrDefault().ShiftBreakTypeDisplayValue;
                    shiftDTO.ShiftBreakDTOs.Add(shiftBreakDTO);
                }
            }

            return shiftDTO;
        }

        #endregion

        #region Update Entity Shift

        /// <summary>
        /// Updates a shift instance in the database
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        public async Task<BaseResponse<bool>> UpdateEntityShift(ShiftDTO shift)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = ShiftRelatedMessages.UpdateShiftUnexpectedError;

            if (shift != null)
            {
                Shift shiftInstance = await _unitOfWork.ShiftRepository.GetById(shift.ShiftId);
                if (shiftInstance == null)
                {
                    response.Message = ShiftRelatedMessages.UpdateShiftNotFound;
                    return response;
                }

                // No destination entity
                else if (shift.EntityId == Guid.Empty)
                {
                    response.Message = ShiftRelatedMessages.ShiftEntityIdIsNull;
                    return response;
                }

                // Name is Empty
                else if (string.IsNullOrEmpty(shift.ShiftName))
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftNameEmpty;
                    return response;
                }

                // No shift duration
                else if (shift.ShiftDuration == TimeSpan.Zero)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftDurationIsNull;
                    return response;
                }

                // Get entity and check ifs null
                Entity destinationEntity = await _unitOfWork.GetGenericRepository<Entity>().GetById(shift.EntityId);
                if (destinationEntity == null)
                {
                    response.Message = ShiftRelatedMessages.EntityNotFound;
                    return response;
                }

                _mapper.Map(shift, shiftInstance);
                await _unitOfWork.ShiftRepository.Update(shiftInstance);
                response.Success = true;
                response.Message = ShiftRelatedMessages.ShiftUpdatedSuccessfuly;

            }

            return response;
        }

        #endregion

        #region Update Entity Shift Break

        /// <summary>
        /// Updates a shift break instance
        /// </summary>
        /// <param name="shiftBreak"></param>
        /// <returns></returns>
        public async Task<BaseResponse<bool>> UpdateEntityShiftBreak(ShiftBreakDTO shiftBreak)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = ShiftRelatedMessages.UpdateShfitBreakUnexpectedError;

            if (shiftBreak != null)
            {
                ShiftBreak shiftBreakInstance = await _unitOfWork.ShiftBreakRepository.GetById(shiftBreak.ShiftParentId);
                if (shiftBreak == null)
                {
                    response.Message = ShiftRelatedMessages.DeleteShiftBreaksNotFound;
                    return response;
                }

                else if (shiftBreak.ShiftParentId == Guid.Empty)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftBreakEmptyShift;
                    return response;
                }

                else if (shiftBreak.ShiftBreakTypeId == 0)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftBreakEmptyShift;
                    return response;
                }

                ShiftBreakType shiftBreakType = await _unitOfWork.GetGenericRepository<ShiftBreakType>().GetById(shiftBreak.ShiftBreakTypeId);

                if (shiftBreakType == null)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftBreakBreakTypeNotFound;
                    return response;
                }

                else if (shiftBreak.ShiftBreakDuration == TimeSpan.Zero)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftBreakDurationIsNull;
                    return response;
                }

                shiftBreakInstance = _mapper.Map<ShiftBreak>(shiftBreak);
                await _unitOfWork.ShiftBreakRepository.Update(shiftBreakInstance);
                response.Success = true;
                response.Message = ShiftRelatedMessages.UpdateShiftBreakSuccessfuly;
            }

            return response;
        }

        #endregion

        #region Get Specific Shifts

        /// <summary>
        /// Gets specific shifts data based on a list of shift identifiers 
        /// </summary>
        /// <param name="shiftIdentifiers"></param>
        /// <returns></returns>
        public async Task<List<ShiftDTO>> GetSpecificShifts(List<string> shiftIdentifiers)
        {
            List<ShiftDTO> shiftDTOs = new List<ShiftDTO>();

            if (shiftIdentifiers.Count != 0)
            {
                List<Shift> shifts = (List<Shift>)await _unitOfWork.ShiftRepository.GetEntityShifts(shiftIdentifiers);

                foreach (Shift shift in shifts)
                {
                    ShiftDTO shiftDTO = _mapper.Map<ShiftDTO>(shift);
                    shiftDTOs.Add(shiftDTO);
                }
            }

            return shiftDTOs;
        }

        #endregion

        #region Get Entity Shift Rotations

        /// <summary>
        /// Gets all Shift Rotation instances from a specific entity
        /// </summary>
        /// <param name="entityId">Entity identifier</param>
        /// <returns>List of Shift Rotation</returns>
        public async Task<List<EntityShiftRotationDTO>> GetEntityShiftRotations(Guid entityId)
        {
            List<EntityShiftRotationDTO> entityShiftRotationDTOs = new List<EntityShiftRotationDTO>();
            if (entityId != Guid.Empty)
            {
                List<EntityShiftRotation> entityShiftRotations = (List<EntityShiftRotation>)await _unitOfWork.EntityShiftRotationRepository.GetEntityShiftsRotation(entityId);
                foreach (EntityShiftRotation entityShiftRotation in entityShiftRotations)
                {
                    EntityShiftRotationDTO entityShiftRotationDTO = _mapper.Map<EntityShiftRotationDTO>(entityShiftRotation);

                    if (entityShiftRotation.IsLeave)
                    {
                        entityShiftRotationDTO.DisplayName = ShiftRelatedMessages.LeaveNAPlaceholder;
                        entityShiftRotationDTO.Alias = ShiftRelatedMessages.LeaveNAAlias;
                        entityShiftRotationDTO.LeaveDurationText = entityShiftRotationDTO.LeaveDuration.ToString();
                    }
                    else
                    {
                        // Use navigation property instead of querying - Shift is already loaded via Include
                        if (entityShiftRotation.Shift != null)
                        {
                            entityShiftRotationDTO.DisplayName = entityShiftRotation.Shift.ShiftName;
                            entityShiftRotationDTO.Alias = entityShiftRotation.Shift.ShiftAlias;
                        }
                    }
                    entityShiftRotationDTOs.Add(entityShiftRotationDTO);
                }
            }
            return entityShiftRotationDTOs;
        }

        #endregion

        #region Add Shift Rotation

        /// <summary>
        /// Adds a new Shift Rotation entry
        /// </summary>
        /// <param name="rotationDTO"></param>
        /// <returns></returns>
        public async Task<BaseResponse<EntityShiftRotationDTO>> AddShiftRotation(AddShiftRotationDTO rotationDTO)
        {
            BaseResponse<EntityShiftRotationDTO> response = new BaseResponse<EntityShiftRotationDTO>();
            response.Success = false;
            response.Message = SharedMessages.UnexpectedError;

            if (rotationDTO != null)
            {
                // Entity required
                if (rotationDTO.EntityId == Guid.Empty)
                {
                    response.Message = ShiftRelatedMessages.ShiftEntityIdIsNull;
                    return response;
                }

                // Shift identifier required when not leave rotation 
                else if (!rotationDTO.IsLeave && rotationDTO.ShiftId == Guid.Empty)
                {
                    response.Message = ShiftRelatedMessages.ShiftIdIsNull;
                    return response;
                }

                else if (rotationDTO.IsLeave && string.IsNullOrEmpty(rotationDTO.LeaveDuration))
                {
                    response.Message = ShiftRelatedMessages.LeaveDurationIsZero;
                    return response;
                }

                // Check if entity exists
                Entity assignedEntity = await _unitOfWork.GetGenericRepository<Entity>().GetById(rotationDTO.EntityId);
                if (assignedEntity == null)
                {
                    response.Message = ShiftRelatedMessages.EntityNotFound;
                    return response;
                }

                Shift assignedShift = null;
                // If its not leave, check if shift exists
                if (!rotationDTO.IsLeave)
                {
                    assignedShift = await _unitOfWork.ShiftRepository.GetById(rotationDTO.ShiftId);
                    if (assignedShift == null)
                    {
                        response.Message = ShiftRelatedMessages.ShiftNotFound;
                        return response;
                    }
                }

                // Get order numbers
                List<int> assignedOrders = await _unitOfWork.EntityShiftRotationRepository.GetAssignedOrderNumbersByEntityId(rotationDTO.EntityId);

                // Set new order number
                EntityShiftRotation entityShiftRotation = _mapper.Map<EntityShiftRotation>(rotationDTO);
                if (assignedOrders.Count == 0)
                    entityShiftRotation.OrderNo = 1;
                else
                    entityShiftRotation.OrderNo = assignedOrders.Max() + 1;

                entityShiftRotation.LeaveDuration = ReturnDuration(rotationDTO.LeaveDuration).Ticks;

                // Add rotation
                entityShiftRotation = await _unitOfWork.EntityShiftRotationRepository.Add(entityShiftRotation);

                // Map relevant information
                EntityShiftRotationDTO entityShiftRotationDTO = _mapper.Map<EntityShiftRotationDTO>(entityShiftRotation);
                if (entityShiftRotationDTO.IsLeave)
                {
                    entityShiftRotationDTO.DisplayName = ShiftRelatedMessages.LeaveNAPlaceholder;
                    entityShiftRotationDTO.Alias = ShiftRelatedMessages.LeaveNAAlias;
                }
                else
                {
                    entityShiftRotationDTO.DisplayName = assignedShift.ShiftName;
                    entityShiftRotationDTO.Alias = assignedShift.ShiftAlias;
                }

                response.Success = true;
                response.Message = ShiftRelatedMessages.AddedToRotationSuccess;
                response.Result = entityShiftRotationDTO;
            }

            return response;
        }

        #endregion

        #region Delete Shift Rotation

        public async Task<BaseResponse<bool>> DeleteShiftRotation(EntityShiftRotationDTO shiftRotationDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Message = SharedMessages.UnexpectedError;

            if (shiftRotationDTO == null)
            {
                response.Message = ShiftRelatedMessages.ShiftRotationDataNull;
                return response;
            }

            else if (shiftRotationDTO.EntityId == Guid.Empty)
            {
                response.Message = ShiftRelatedMessages.ShiftEntityIdIsNull;
                return response;
            }

            else if (shiftRotationDTO.OrderNo == 0)
            {
                response.Message = ShiftRelatedMessages.ShiftRotationBadOrderNumber;
                return response;
            }

            List<EntityShiftRotation> entityShiftRotations = await _unitOfWork.EntityShiftRotationRepository.GetEntityShiftsRotation(shiftRotationDTO.EntityId);
            EntityShiftRotation entityShiftRotation = entityShiftRotations.Where(i => i.ShiftId.Equals(shiftRotationDTO.ShiftId) && i.OrderNo.Equals(shiftRotationDTO.OrderNo)).FirstOrDefault();
            if (entityShiftRotation == null)
            {
                response.Message = ShiftRelatedMessages.ShiftRotationNotFound;
                return response;
            }
            else
            {
                try
                {
                    await _unitOfWork.BeginTransactionAsync();

                    await _unitOfWork.EntityShiftRotationRepository.DeleteEntityShiftRotation(entityShiftRotation);

                    await _unitOfWork.SaveChangesAsync();

                    entityShiftRotations.Remove(entityShiftRotation);

                    int order = 1;

                    foreach (EntityShiftRotation shiftRotation in entityShiftRotations)
                    {
                        if (shiftRotation.OrderNo != order)
                        {
                            await _unitOfWork.EntityShiftRotationRepository.DeleteEntityShiftRotation(shiftRotation);
                            await _unitOfWork.SaveChangesAsync();

                            shiftRotation.OrderNo = order;
                            await _unitOfWork.EntityShiftRotationRepository.Add(shiftRotation);
                        }
                        order++;
                    }

                    await _unitOfWork.CommitAsync();

                    response.Message = ShiftRelatedMessages.ShiftRotationDeletedSuccess;
                    response.Success = true;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                    return response;
                }
            }

            return response;
        }

        #endregion

        #region Update Entity Shift Rotation Order

        public async Task<BaseResponse<bool>> UpdateEntityShiftRotationOrder(UpdateShiftRotationDTO shiftRotationDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = SharedMessages.UnexpectedError;

            if (shiftRotationDTO == null)
            {
                response.Message = ShiftRelatedMessages.ShiftRotationIsNull;
                return response;
            }

            EntityShiftRotation entityShiftRotation = await _unitOfWork.EntityShiftRotationRepository.GetEntityShiftRotation(shiftRotationDTO.EntityId, shiftRotationDTO.OrderNo);
            if (entityShiftRotation == null)
            {
                response.Message = ShiftRelatedMessages.ShiftRotationNotFound;
                return response;
            }

            if (shiftRotationDTO.OrderNo == 0 || shiftRotationDTO.NewOrderNo == 0)
            {
                response.Message = ShiftRelatedMessages.ShiftRotationBadOrderNumber;
                return response;
            }

            // Change in Order number
            if (shiftRotationDTO.OrderNo != shiftRotationDTO.NewOrderNo)
            {
                try
                {
                    await _unitOfWork.BeginTransactionAsync();

                    List<EntityShiftRotation> entityShiftRotations = await _unitOfWork.EntityShiftRotationRepository.GetEntityShiftsRotation(shiftRotationDTO.EntityId);

                    EntityShiftRotation destinationShiftRotation = entityShiftRotations.Where(i => i.OrderNo.Equals(shiftRotationDTO.NewOrderNo)).FirstOrDefault();
                    EntityShiftRotation sourceShiftRotation = entityShiftRotations.Where(i => i.OrderNo.Equals(shiftRotationDTO.OrderNo)).FirstOrDefault();

                    if (sourceShiftRotation == null || destinationShiftRotation == null)
                    {
                        response.Message = ShiftRelatedMessages.ShiftRotationBadOrderNumber;
                        return response;
                    }

                    await _unitOfWork.EntityShiftRotationRepository.DeleteEntityShiftRotation(destinationShiftRotation);
                    await _unitOfWork.EntityShiftRotationRepository.DeleteEntityShiftRotation(sourceShiftRotation);

                    await _unitOfWork.SaveChangesAsync();

                    destinationShiftRotation.OrderNo = shiftRotationDTO.OrderNo;
                    sourceShiftRotation.OrderNo = shiftRotationDTO.NewOrderNo;

                    await _unitOfWork.EntityShiftRotationRepository.Add(destinationShiftRotation);
                    await _unitOfWork.EntityShiftRotationRepository.Add(sourceShiftRotation);

                    await _unitOfWork.CommitAsync();
                    response.Success = true;
                    response.Message = ShiftRelatedMessages.UpdateShiftRotationSucess;
                }
                catch (Exception ex)
                {
                    await _unitOfWork.RollbackAsync();
                    return response;
                }
            }

            return response;
        }

        #endregion

        #region Update Shift Rotation

        public async Task<BaseResponse<bool>> UpdateEntityShiftRotation(EntityShiftRotationDTO shiftRotationDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = SharedMessages.UnexpectedError;

            if (shiftRotationDTO == null)
            {
                response.Message = ShiftRelatedMessages.ShiftRotationIsNull;
                return response;
            }

            EntityShiftRotation entityShiftRotation = await _unitOfWork.EntityShiftRotationRepository.GetEntityShiftRotation(shiftRotationDTO.EntityId, shiftRotationDTO.OrderNo);
            if (entityShiftRotation == null)
            {
                response.Message = ShiftRelatedMessages.ShiftRotationNotFound;
                return response;
            }

            try
            {
                entityShiftRotation.IsLeave = shiftRotationDTO.IsLeave;
                entityShiftRotation.ShiftId = shiftRotationDTO.ShiftId;
                entityShiftRotation.LeaveDuration = ReturnDuration(shiftRotationDTO.LeaveDurationText).Ticks;

                await _unitOfWork.EntityShiftRotationRepository.Update(entityShiftRotation);
                response.Success = true;
                response.Message = ShiftRelatedMessages.UpdateShiftRotationSucess;
            }
            catch (Exception ex)
            {
                string strErr = ex.Message;
            }

            return response;
        }

        #endregion

        #region Return Duration

        private TimeSpan ReturnDuration(string dateString)
        {
            TimeSpan duration = TimeSpan.Zero;
            if (!string.IsNullOrEmpty(dateString))
            {
                string[] daySplit;

                if (dateString.Contains('.'))
                    daySplit = dateString.Split('.');

                else
                    daySplit = dateString.Split(' ');


                if (daySplit.Length == 2)
                {
                    string[] hourMinuteSplit = daySplit[1].Split(':');
                    duration = new TimeSpan(int.Parse(daySplit[0]), int.Parse(hourMinuteSplit[0]), int.Parse(hourMinuteSplit[1]), 0);
                }
                else
                {
                    string[] hourMinuteSplit = dateString.Split(':');
                    duration = new TimeSpan(int.Parse(hourMinuteSplit[0]), int.Parse(hourMinuteSplit[1]), 0);
                }
            }
            return duration;
        }

        #endregion

        #region Get Total Entity Shifts

        public async Task<int> GetTotalEntityShifts(Guid entityId)
        {
            int count = 0;

            if(entityId != Guid.Empty)
            {
                IEnumerable<Shift> entityShifts = await _unitOfWork.ShiftRepository.GetEntityShifts(entityId);
                count = entityShifts.Count();
            }

            return count;
        }

        #endregion

        #endregion
    }
}
