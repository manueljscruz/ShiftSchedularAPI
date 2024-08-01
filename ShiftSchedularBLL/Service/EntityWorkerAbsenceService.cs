using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.AbsenceManagement;

namespace ShiftSchedularBLL.Service
{
    public class EntityWorkerAbsenceService : IEntityWorkerAbsenceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAbsenceTypeLocalizationRepository _absenceTypeLocalizationRepository;
        private readonly IEntityWorkerAbsenceRepository _entityWorkerAbsenceRepository;
        private readonly ILocalizationRepository _localizationRepository;
        private readonly IGeneralService _generalService;
        private readonly IWorkerRepository _workerRepository;
        private readonly IGenericRepository<Entity> _entityRepository;
        private readonly IEntityWorkerRepository _entityWorkerRepository;
        private readonly IMapper _mapper;

        #region Constructor

        public EntityWorkerAbsenceService(IUnitOfWork unitOfWork,
           IAbsenceTypeLocalizationRepository absenceTypeLocalizationRepository,
           IEntityWorkerAbsenceRepository entityWorkerAbsenceRepository,
           ILocalizationRepository localizationRepository,
           IGeneralService generalService,
           IWorkerRepository workerRepository,
           IGenericRepository<Entity> entityRepository,
           IEntityWorkerRepository entityWorkerRepository,
           IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _absenceTypeLocalizationRepository = absenceTypeLocalizationRepository;
            _entityWorkerAbsenceRepository = entityWorkerAbsenceRepository;
            _localizationRepository = localizationRepository;
            _generalService = generalService;
            _workerRepository = workerRepository;
            _entityRepository = entityRepository;
            _entityWorkerRepository = entityWorkerRepository;
            _mapper = mapper;
        }

        #endregion

        #region Methods

        #region Absence Approval Decision

        /// <summary>
        /// Fills fields related to the approval or disapproval of the absence requested by the user
        /// </summary>
        /// <param name="absenceApprovalDecisionDTO"></param>
        /// <returns></returns>
        public async Task<BaseResponse<EntityWorkerAbsenceDTO>> AbsenceApprovalDecision(AbsenceApprovalDecisionDTO absenceApprovalDecisionDTO)
        {
            BaseResponse<EntityWorkerAbsenceDTO> response = new BaseResponse<EntityWorkerAbsenceDTO>();
            response.Success = false;
            response.Message = AbsenceRelatedMessages.AbsenceDecisionUnexpectedError;

            if(absenceApprovalDecisionDTO != null)
            {
                // Absence identifier is empty
                if(absenceApprovalDecisionDTO.EntityWorkerAbsenceId == null)
                {
                    response.Message = AbsenceRelatedMessages.AbsenceIdIsNull;
                    return response;
                }

                // Get Absence instance
                EntityWorkerAbsence entityWorkerAbsence = await _entityWorkerAbsenceRepository.GetById(absenceApprovalDecisionDTO.EntityWorkerAbsenceId);
                if(entityWorkerAbsence == null)
                {
                    response.Message = AbsenceRelatedMessages.AbsenceNotFound;
                    return response;
                }

                // No signature associated from the decision approval
                if (string.IsNullOrEmpty(absenceApprovalDecisionDTO.AbsenceDecisionSignature))
                {
                    response.Message = AbsenceRelatedMessages.AbsenceDecisionApproverIsNull;
                    return response;
                }

                // Check if member is not eligible to perform this decision
                else if (!await _entityWorkerRepository.IsMemberOwner(entityWorkerAbsence.EntityId, absenceApprovalDecisionDTO.AbsenceDecisionSignature))
                {
                    response.Message = AbsenceRelatedMessages.AbsenceDecisionApproverIsNotOwner;
                    return response;
                }

                // Assign values and update absence entry
                entityWorkerAbsence.AbsenceApproved = absenceApprovalDecisionDTO.AbsenceDecision;
                entityWorkerAbsence.AbsenceDecisionOwner = absenceApprovalDecisionDTO.AbsenceDecisionSignature;
                entityWorkerAbsence.AbsenceDateDecisionOffset = new DateTimeOffset(DateTime.Now).Offset;
                entityWorkerAbsence.AbsenceDateDecision = DateTime.UtcNow;

                await _entityWorkerAbsenceRepository.Update(entityWorkerAbsence);

                response.Result = await this.GetEntityWorkerAbsenceById(entityWorkerAbsence.EntityWorkerAbsenceId, absenceApprovalDecisionDTO.LanguageCode);
                response.Success = true;
                response.Message = AbsenceRelatedMessages.AbsenceDecisionApprovalSubmitted;
            }

            return response;
        }

        #endregion

        #region Add Entity Worker Absence

        /// <summary>
        /// Adds a new absence instance
        /// </summary>
        /// <param name="addEntityWorkerAbsence"></param>
        /// <returns></returns>
        public async Task<BaseResponse<EntityWorkerAbsenceDTO>> AddEntityWorkerAbsence(AddEntityWorkerAbsenceDTO addEntityWorkerAbsence)
        {
            BaseResponse<EntityWorkerAbsenceDTO> response = new BaseResponse<EntityWorkerAbsenceDTO>();
            response.Success = false;
            response.Message = AbsenceRelatedMessages.AddEntityWorkerAbsenceUnexpectedError;

            if(addEntityWorkerAbsence != null)
            {
                // Validate
                if(string.IsNullOrEmpty(addEntityWorkerAbsence.WorkerId))
                {
                    response.Message = AbsenceRelatedMessages.WorkerIdIsEmpty;
                    return response;
                }

                else if(await _workerRepository.GetById(addEntityWorkerAbsence.WorkerId) == null)
                {
                    response.Message = AbsenceRelatedMessages.WorkerNotFound;
                    return response;
                }

                else if(string.IsNullOrEmpty(addEntityWorkerAbsence.EntityId))
                {
                    response.Message = AbsenceRelatedMessages.EntityIdIsEmpty;
                    return response;
                }

                else if (await _entityRepository.GetById(addEntityWorkerAbsence.EntityId) == null)
                {
                    response.Message = AbsenceRelatedMessages.EntityNotFound;
                    return response;
                }

                else if(addEntityWorkerAbsence.AbsenceTypeId == 0)
                {
                    response.Message = AbsenceRelatedMessages.AbsenceTypeIsInvalid;
                    return response;
                }

                else if(addEntityWorkerAbsence.AbsenceStartDate == new DateTime())
                {
                    response.Message = AbsenceRelatedMessages.AbsenceStartDateEmpty;
                    return response;
                }

                else if(addEntityWorkerAbsence.AbsenceEndDate == new DateTime())
                {
                    response.Message = AbsenceRelatedMessages.AbsenceEndDateEmpty;
                    return response;
                }

                else if(addEntityWorkerAbsence.AbsenceEndDate < addEntityWorkerAbsence.AbsenceStartDate)
                {
                    response.Message = AbsenceRelatedMessages.AbsenceDatesInvalidInterval;
                    return response;
                }

                // Map add instance to entity instance
                EntityWorkerAbsence entityWorkerAbsence = _mapper.Map<EntityWorkerAbsence>(addEntityWorkerAbsence);

                // Add Id and Dates in Universal Time
                entityWorkerAbsence.EntityWorkerAbsenceId = _generalService.GenerateGuid();
                entityWorkerAbsence.AbsenceStartDate = entityWorkerAbsence.AbsenceStartDate.ToUniversalTime();
                entityWorkerAbsence.AbsenceEndDate = entityWorkerAbsence.AbsenceEndDate.ToUniversalTime();
                entityWorkerAbsence.DateOffset = new DateTimeOffset(entityWorkerAbsence.AbsenceStartDate).Offset;
                entityWorkerAbsence.AbsenceDecisionOwner = string.Empty;

                // Add Instance
                try
                {
                    entityWorkerAbsence = await _entityWorkerAbsenceRepository.Add(entityWorkerAbsence);
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    return response;
                }
                

                // Get Localized Absence types
                IEnumerable<AbsenceTypeLocalization> absenceTypeLocalizeds = await _absenceTypeLocalizationRepository.GetAbsenceTypesByLocalization(addEntityWorkerAbsence.LanguageCode);

                // Map added object to DTO instance
                EntityWorkerAbsenceDTO entityWorkerAbsenceDTO = await HandleEntityWorkerAbsenceData(entityWorkerAbsence, absenceTypeLocalizeds);

                response.Result = entityWorkerAbsenceDTO;
                response.Success = true;
                response.Message = AbsenceRelatedMessages.AddEntityWorkerAbsenceSuccessMessage;
            }

            return response;
        }

        #endregion

        #region Delete Entity Worker Absence

        /// <summary>
        /// Deletes an absence instance
        /// </summary>
        /// <param name="absenceId"></param>
        /// <returns></returns>
        public async Task<BaseResponse<bool>> DeleteEntityWorkerAbsence(string absenceId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = AbsenceRelatedMessages.DeleteEntityWorkerAbsenceUnexpectedError;

            if(string.IsNullOrWhiteSpace(absenceId))
            {
                response.Message = AbsenceRelatedMessages.AbsenceIdIsNull;
                return response;
            }

            EntityWorkerAbsence entityWorkerAbsence = await _entityWorkerAbsenceRepository.GetById(absenceId);
            if(entityWorkerAbsence == null)
            {
                response.Message = AbsenceRelatedMessages.AbsenceNotFound;
                return response;
            }
            else
            {
                await _entityWorkerAbsenceRepository.Delete(entityWorkerAbsence.EntityWorkerAbsenceId);
                response.Success = true;
                response.Message = AbsenceRelatedMessages.DeleteEntityWorkerAbsenceSuccessMessage;
            }

            return response;
        }

        #endregion

        #region Get Entity Worker Absence By Id

        /// <summary>
        /// Gets an absence instance by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lcode"></param>
        /// <returns></returns>
        public async Task<EntityWorkerAbsenceDTO> GetEntityWorkerAbsenceById(string id, string lcode)
        {
            if (!string.IsNullOrEmpty(id))
            {
                EntityWorkerAbsence entityWorkerAbsence = await _entityWorkerAbsenceRepository.GetById(id);

                // Get Localized Absence types
                IEnumerable<AbsenceTypeLocalization> absenceTypeLocalizeds = await _absenceTypeLocalizationRepository.GetAbsenceTypesByLocalization(lcode);

                // Map added object to DTO instance
                EntityWorkerAbsenceDTO entityWorkerAbsenceDTO = await HandleEntityWorkerAbsenceData(entityWorkerAbsence, absenceTypeLocalizeds);

                return entityWorkerAbsenceDTO;
            }
            return null;
        }

        #endregion

        #region Handle Entity Worker Absence Data

        /// <summary>
        /// Handles the absence data and parses it to a transferable object instance
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="absenceTypeLocalizeds"></param>
        /// <returns></returns>
        private async Task<EntityWorkerAbsenceDTO> HandleEntityWorkerAbsenceData(EntityWorkerAbsence instance, IEnumerable<AbsenceTypeLocalization> absenceTypeLocalizeds)
        {
            EntityWorkerAbsenceDTO entityWorkerAbsenceDTO = _mapper.Map<EntityWorkerAbsenceDTO>(instance);

            AbsenceTypeLocalization absenceType = absenceTypeLocalizeds.Where(i => i.AbsenceTypeId.Equals(entityWorkerAbsenceDTO.AbsenceTypeId)).FirstOrDefault();
            if (absenceType != null)
                entityWorkerAbsenceDTO.AbsenceTypeDisplayValue = absenceType.AbsenceTypeDisplayValue;

            if (!string.IsNullOrEmpty(entityWorkerAbsenceDTO.AbsenceDecisionOwner))
            {
                Worker approver = await _workerRepository.GetById(entityWorkerAbsenceDTO.AbsenceDecisionOwner);
                entityWorkerAbsenceDTO.AbsenceApproverName = approver.WorkerName;
            }

            return entityWorkerAbsenceDTO;
        }

        #endregion

        #region Get Entity Worker Absence View Model

        /// <summary>
        /// Gets the required data for the absence view model
        /// </summary>
        /// <param name="viewModelRequestDTO"></param>
        /// <returns></returns>
        public async Task<EntityWorkerAbsenceViewModel> GetEntityWorkerAbsenceViewModel(BaseViewModelRequest viewModelRequestDTO)
        {
            EntityWorkerAbsenceViewModel viewModel = new EntityWorkerAbsenceViewModel();

            if(viewModelRequestDTO != null && !string.IsNullOrEmpty(viewModelRequestDTO.EntityId) && !string.IsNullOrEmpty(viewModelRequestDTO.WorkerId) && !string.IsNullOrEmpty(viewModelRequestDTO.LanguageCode))
            {
                // Check if its the owner
                viewModel.IsOwner = await _entityWorkerRepository.IsMemberOwner(viewModelRequestDTO.EntityId, viewModelRequestDTO.WorkerId);

                // Retrieve all the absence types
                IEnumerable<AbsenceTypeLocalization> absenceTypeLocalizeds = await _absenceTypeLocalizationRepository.GetAbsenceTypesByLocalization(viewModelRequestDTO.LanguageCode);
                foreach(AbsenceTypeLocalization absenceType in absenceTypeLocalizeds)
                    viewModel.AbsenceTypeLocalizeds.Add(_mapper.Map<AbsenceTypeLocalizedDTO>(absenceType));

                // Gets the entity worker absences of everyone if it is the owner, otherwise only of the user requesting it
                IEnumerable<EntityWorkerAbsence> entityWorkerAbsences = await _entityWorkerAbsenceRepository.GetEntityWorkerAbsences(viewModelRequestDTO.EntityId, viewModelRequestDTO.WorkerId, viewModel.IsOwner);
                foreach(EntityWorkerAbsence entityWorkerAbsence in entityWorkerAbsences)
                    viewModel.EntityWorkerAbsences.Add(await HandleEntityWorkerAbsenceData(entityWorkerAbsence, absenceTypeLocalizeds));
                
            }

            return viewModel;
        }

        #endregion

        #region Update Entity Worker Absence

        /// <summary>
        /// Updates an absence instance
        /// </summary>
        /// <param name="entityWorkerAbsenceDTO"></param>
        /// <returns></returns>
        public async Task<BaseResponse<EntityWorkerAbsenceDTO>> UpdateEntityWorkerAbsence(EntityWorkerAbsenceDTO entityWorkerAbsenceDTO)
        {
            BaseResponse<EntityWorkerAbsenceDTO> response = new BaseResponse<EntityWorkerAbsenceDTO>();
            response.Success = false;
            response.Message = AbsenceRelatedMessages.UpdateEntityWorkerAbsenceUnexpectedError;

            if(entityWorkerAbsenceDTO != null)
            {
                if (string.IsNullOrEmpty(entityWorkerAbsenceDTO.EntityWorkerAbsenceId))
                {
                    response.Message = AbsenceRelatedMessages.AbsenceIdIsNull;
                    return response;
                }

                EntityWorkerAbsence entityWorkerAbsence = await _entityWorkerAbsenceRepository.GetById(entityWorkerAbsenceDTO.EntityWorkerAbsenceId);
                if (entityWorkerAbsence == null)
                {
                    response.Message = AbsenceRelatedMessages.AbsenceNotFound;
                    return response;
                }

                else if (string.IsNullOrEmpty(entityWorkerAbsenceDTO.WorkerId))
                {
                    response.Message = AbsenceRelatedMessages.WorkerIdIsEmpty;
                    return response;
                }

                else if (await _workerRepository.GetById(entityWorkerAbsenceDTO.WorkerId) == null)
                {
                    response.Message = AbsenceRelatedMessages.WorkerNotFound;
                    return response;
                }

                else if (string.IsNullOrEmpty(entityWorkerAbsenceDTO.EntityId))
                {
                    response.Message = AbsenceRelatedMessages.EntityIdIsEmpty;
                    return response;
                }

                else if (await _entityRepository.GetById(entityWorkerAbsenceDTO.EntityId) == null)
                {
                    response.Message = AbsenceRelatedMessages.EntityNotFound;
                    return response;
                }

                else if (entityWorkerAbsenceDTO.AbsenceTypeId == 0)
                {
                    response.Message = AbsenceRelatedMessages.AbsenceTypeIsInvalid;
                    return response;
                }

                else if (entityWorkerAbsenceDTO.AbsenceStartDate == new DateTime())
                {
                    response.Message = AbsenceRelatedMessages.AbsenceStartDateEmpty;
                    return response;
                }

                else if (entityWorkerAbsenceDTO.AbsenceEndDate == new DateTime())
                {
                    response.Message = AbsenceRelatedMessages.AbsenceEndDateEmpty;
                    return response;
                }

                else if (entityWorkerAbsenceDTO.AbsenceEndDate < entityWorkerAbsenceDTO.AbsenceStartDate)
                {
                    response.Message = AbsenceRelatedMessages.AbsenceDatesInvalidInterval;
                    return response;
                }

                _mapper.Map(entityWorkerAbsenceDTO, entityWorkerAbsence);
                entityWorkerAbsence.DateOffset = new DateTimeOffset(entityWorkerAbsenceDTO.AbsenceStartDate).Offset;

                // if there is an previous approval decision, reset it
                if (entityWorkerAbsence.AbsenceDecisionOwner != null && entityWorkerAbsence.AbsenceDateDecision != new DateTime())
                {
                    entityWorkerAbsence.AbsenceDecisionOwner = string.Empty;
                    entityWorkerAbsence.AbsenceDateDecision = new DateTime();
                    entityWorkerAbsence.AbsenceDateDecisionOffset = new TimeSpan();
                    entityWorkerAbsence.AbsenceApproved = false;
                }

                try
                {
                    await _entityWorkerAbsenceRepository.Update(entityWorkerAbsence);

                    response.Result = _mapper.Map(entityWorkerAbsence, entityWorkerAbsenceDTO);
                    response.Success = true;
                    response.Message = AbsenceRelatedMessages.UpdateEntityWorkerAbsenceSuccessMessage;
                }
                catch (Exception ex)
                {
                    string strError = ex.Message;
                }
            }

            return response;
        }

        #endregion

        #endregion
    }
}
