using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
        private readonly IGeneralService _generalService;
        private readonly IMapper _mapper;
        private readonly UserManager<ApplicationUser> _userManager;

        #region Constructor

        public EntityWorkerAbsenceService(IUnitOfWork unitOfWork,
           IGeneralService generalService,
           UserManager<ApplicationUser> userManager,
           IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _generalService = generalService;
            _userManager = userManager;
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

            if (absenceApprovalDecisionDTO != null)
            {
                // Absence identifier is empty
                if (absenceApprovalDecisionDTO.EntityWorkerAbsenceId == Guid.Empty)
                {
                    response.Message = AbsenceRelatedMessages.AbsenceIdIsNull;
                    return response;
                }

                // Get Absence instance
                EntityWorkerAbsence entityWorkerAbsence = await _unitOfWork.EntityWorkerAbsenceRepository.GetById(absenceApprovalDecisionDTO.EntityWorkerAbsenceId);
                if (entityWorkerAbsence == null)
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
                else if (!await _unitOfWork.EntityWorkerRepository.IsMemberOwner(entityWorkerAbsence.EntityId, absenceApprovalDecisionDTO.AbsenceDecisionSignature))
                {
                    response.Message = AbsenceRelatedMessages.AbsenceDecisionApproverIsNotOwner;
                    return response;
                }

                // Assign values and update absence entry
                entityWorkerAbsence.AbsenceApproved = absenceApprovalDecisionDTO.AbsenceDecision;
                entityWorkerAbsence.AbsenceDecisionOwner = absenceApprovalDecisionDTO.AbsenceDecisionSignature;
                entityWorkerAbsence.AbsenceDateDecisionOffset = GetTimeZoneOffsetMinutes(absenceApprovalDecisionDTO.DecisionTimezoneId);
                entityWorkerAbsence.AbsenceDateDecision = DateTime.UtcNow;

                await _unitOfWork.EntityWorkerAbsenceRepository.Update(entityWorkerAbsence);

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

            if (addEntityWorkerAbsence != null)
            {
                // Validate
                if (string.IsNullOrEmpty(addEntityWorkerAbsence.WorkerId))
                {
                    response.Message = AbsenceRelatedMessages.WorkerIdIsEmpty;
                    return response;
                }

                else if (await _userManager.FindByIdAsync(addEntityWorkerAbsence.WorkerId) == null)
                {
                    response.Message = AbsenceRelatedMessages.WorkerNotFound;
                    return response;
                }

                else if (addEntityWorkerAbsence.EntityId == Guid.Empty)
                {
                    response.Message = AbsenceRelatedMessages.EntityIdIsEmpty;
                    return response;
                }

                else if (await _unitOfWork.GetGenericRepository<Entity>().GetById(addEntityWorkerAbsence.EntityId) == null)
                {
                    response.Message = AbsenceRelatedMessages.EntityNotFound;
                    return response;
                }

                else if (addEntityWorkerAbsence.AbsenceTypeId == 0)
                {
                    response.Message = AbsenceRelatedMessages.AbsenceTypeIsInvalid;
                    return response;
                }

                else if (addEntityWorkerAbsence.AbsenceStartDate == new DateTime())
                {
                    response.Message = AbsenceRelatedMessages.AbsenceStartDateEmpty;
                    return response;
                }

                else if (addEntityWorkerAbsence.AbsenceEndDate == new DateTime())
                {
                    response.Message = AbsenceRelatedMessages.AbsenceEndDateEmpty;
                    return response;
                }

                else if (addEntityWorkerAbsence.AbsenceEndDate < addEntityWorkerAbsence.AbsenceStartDate)
                {
                    response.Message = AbsenceRelatedMessages.AbsenceDatesInvalidInterval;
                    return response;
                }

                EntityWorkerAbsence entityWorkerAbsence = null;

                // Map add instance to entity instance
                entityWorkerAbsence = _mapper.Map<EntityWorkerAbsence>(addEntityWorkerAbsence);

                // Add Id and Dates in Universal Time
                entityWorkerAbsence.EntityWorkerAbsenceId = new Guid();
                if (!entityWorkerAbsence.IsFullDay)
                {
                    entityWorkerAbsence.AbsenceStartDate = entityWorkerAbsence.AbsenceStartDate.ToUniversalTime();
                    entityWorkerAbsence.AbsenceEndDate = entityWorkerAbsence.AbsenceEndDate.ToUniversalTime();
                }
                entityWorkerAbsence.AbsenceDecisionOwner = string.Empty;

                try
                {
                    // Add Instance
                    entityWorkerAbsence = await _unitOfWork.EntityWorkerAbsenceRepository.Add(entityWorkerAbsence);
                }
                catch (Exception ex)
                {
                    string error = ex.Message;
                    return response;
                }

                // Map added object to DTO instance
                EntityWorkerAbsenceDTO entityWorkerAbsenceDTO = await HandleEntityWorkerAbsenceData(entityWorkerAbsence, addEntityWorkerAbsence.LanguageCode);

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
        public async Task<BaseResponse<bool>> DeleteEntityWorkerAbsence(Guid absenceId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = AbsenceRelatedMessages.DeleteEntityWorkerAbsenceUnexpectedError;

            if (absenceId == Guid.Empty)
            {
                response.Message = AbsenceRelatedMessages.AbsenceIdIsNull;
                return response;
            }

            EntityWorkerAbsence entityWorkerAbsence = await _unitOfWork.EntityWorkerAbsenceRepository.GetById(absenceId);
            if (entityWorkerAbsence == null)
            {
                response.Message = AbsenceRelatedMessages.AbsenceNotFound;
                return response;
            }
            else
            {
                await _unitOfWork.EntityWorkerAbsenceRepository.Delete(entityWorkerAbsence.EntityWorkerAbsenceId);
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
        public async Task<EntityWorkerAbsenceDTO> GetEntityWorkerAbsenceById(Guid id, string lcode)
        {
            if (id != Guid.Empty)
            {
                EntityWorkerAbsence entityWorkerAbsence = await _unitOfWork.EntityWorkerAbsenceRepository.GetById(id);

                // Get Localized Absence types
                IEnumerable<AbsenceTypeLocalization> absenceTypeLocalizeds = await _unitOfWork.AbsenceTypeLocalizationRepository.GetAbsenceTypesByLocalization(lcode);

                // Map added object to DTO instance
                EntityWorkerAbsenceDTO entityWorkerAbsenceDTO = await HandleEntityWorkerAbsenceData(entityWorkerAbsence, lcode);

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
        private async Task<EntityWorkerAbsenceDTO> HandleEntityWorkerAbsenceData(EntityWorkerAbsence instance, string languageCode)
        {
            EntityWorkerAbsenceDTO entityWorkerAbsenceDTO = _mapper.Map<EntityWorkerAbsenceDTO>(instance);
            // Get Localized Absence types
            IEnumerable<AbsenceTypeLocalization> absenceTypeLocalizeds = await _unitOfWork.AbsenceTypeLocalizationRepository.GetAbsenceTypesByLocalization(languageCode);


            AbsenceTypeLocalization absenceType = absenceTypeLocalizeds.Where(i => i.AbsenceTypeId.Equals(entityWorkerAbsenceDTO.AbsenceTypeId)).FirstOrDefault();
            if (absenceType != null)
                entityWorkerAbsenceDTO.AbsenceTypeDisplayValue = absenceType.AbsenceTypeDisplayValue;

            if (!string.IsNullOrEmpty(entityWorkerAbsenceDTO.AbsenceDecisionOwner))
            {
                ApplicationUser approver = await _userManager.FindByIdAsync(entityWorkerAbsenceDTO.AbsenceDecisionOwner);
                entityWorkerAbsenceDTO.AbsenceApproverName = approver.DisplayName;
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
        public async Task<EntityWorkerAbsenceViewModel> GetEntityWorkerAbsenceViewModel(PagedModelRequest viewModelRequestDTO)
        {
            EntityWorkerAbsenceViewModel viewModel = new EntityWorkerAbsenceViewModel();

            if (viewModelRequestDTO != null && viewModelRequestDTO.EntityId != Guid.Empty && !string.IsNullOrEmpty(viewModelRequestDTO.WorkerId) && !string.IsNullOrEmpty(viewModelRequestDTO.LanguageCode))
            {
                // Check if its the owner
                viewModel.IsOwner = await _unitOfWork.EntityWorkerRepository.IsMemberOwner(viewModelRequestDTO.EntityId, viewModelRequestDTO.WorkerId);

                // Retrieve all the absence types
                IEnumerable<AbsenceTypeLocalization> absenceTypeLocalizeds = await _unitOfWork.AbsenceTypeLocalizationRepository.GetAbsenceTypesByLocalization(viewModelRequestDTO.LanguageCode);
                foreach (AbsenceTypeLocalization absenceType in absenceTypeLocalizeds)
                    viewModel.AbsenceTypeLocalizeds.Add(_mapper.Map<AbsenceTypeLocalizedDTO>(absenceType));

                // Gets the entity worker absences of everyone if it is the owner, otherwise only of the user requesting it
                viewModel.EntityWorkerAbsences = await GetEntityWorkerAbsences(viewModelRequestDTO);
            }

            return viewModel;
        }

        #endregion

        #region Get Entity Worker Absences

        public async Task<PagedList<EntityWorkerAbsenceDTO>> GetEntityWorkerAbsences(PagedModelRequest pagedModelRequest)
        {
            List<EntityWorkerAbsenceDTO> entityWorkerAbsencesList = new List<EntityWorkerAbsenceDTO>();

            EntityWorker entityWorkerInstance = await _unitOfWork.EntityWorkerRepository.GetByWorkerAndEntity(pagedModelRequest.WorkerId, pagedModelRequest.EntityId);

            IEnumerable<EntityWorkerAbsence> entityWorkerAbsences = await _unitOfWork.EntityWorkerAbsenceRepository.GetEntityWorkerAbsences(pagedModelRequest.EntityId, pagedModelRequest.WorkerId, entityWorkerInstance.IsOwner);
            int totalCount = entityWorkerAbsences.Count();

            entityWorkerAbsences = entityWorkerAbsences.OrderBy(i => i.AbsenceStartDate);

            if(pagedModelRequest.NextPage != 0 && pagedModelRequest.ItemsPerPage != 0)
            {
                int skipRows = (pagedModelRequest.NextPage - 1) * pagedModelRequest.ItemsPerPage;

                entityWorkerAbsences = entityWorkerAbsences.Skip(skipRows).Take(pagedModelRequest.ItemsPerPage);
            }

            foreach (EntityWorkerAbsence entityWorkerAbsence in entityWorkerAbsences)
                entityWorkerAbsencesList.Add(await HandleEntityWorkerAbsenceData(entityWorkerAbsence, pagedModelRequest.LanguageCode));

            PagedList<EntityWorkerAbsenceDTO> pagedList = PagedList<EntityWorkerAbsenceDTO>.Create(entityWorkerAbsencesList.AsQueryable(), totalCount, pagedModelRequest.NextPage, pagedModelRequest.ItemsPerPage);

            return pagedList;
        }

        #endregion

        #region Get Specific Worker Absences

        public async Task<List<EntityWorkerAbsenceDTO>> GetSpecificWorkerAbsences(Guid entityId, List<string> workers, string languageCode, DateTime? startDate = null, DateTime? endDate = null)
        {
            List<EntityWorkerAbsenceDTO> entityWorkerAbsenceDTOs = new List<EntityWorkerAbsenceDTO>();

            if(workers.Count != 0)
            {
                IEnumerable<EntityWorkerAbsence> entityWorkerAbsences = await _unitOfWork.EntityWorkerAbsenceRepository.GetSpecificWorkerAbsences(entityId, workers, startDate, endDate);

                foreach (EntityWorkerAbsence entityWorkerAbsence in entityWorkerAbsences)
                    entityWorkerAbsenceDTOs.Add(await HandleEntityWorkerAbsenceData(entityWorkerAbsence, languageCode));
            }

            return entityWorkerAbsenceDTOs;
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

            if (entityWorkerAbsenceDTO != null)
            {
                if (entityWorkerAbsenceDTO.EntityWorkerAbsenceId == Guid.Empty)
                {
                    response.Message = AbsenceRelatedMessages.AbsenceIdIsNull;
                    return response;
                }

                EntityWorkerAbsence entityWorkerAbsence = await _unitOfWork.EntityWorkerAbsenceRepository.GetById(entityWorkerAbsenceDTO.EntityWorkerAbsenceId);
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

                else if (await _userManager.FindByIdAsync(entityWorkerAbsenceDTO.WorkerId) == null)
                {
                    response.Message = AbsenceRelatedMessages.WorkerNotFound;
                    return response;
                }

                else if (entityWorkerAbsenceDTO.EntityId == Guid.Empty)
                {
                    response.Message = AbsenceRelatedMessages.EntityIdIsEmpty;
                    return response;
                }

                else if (await _unitOfWork.GetGenericRepository<Entity>().GetById(entityWorkerAbsenceDTO.EntityId) == null)
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
                entityWorkerAbsence.DateOffset = GetTimeZoneOffsetMinutes(entityWorkerAbsenceDTO.TimezoneId, entityWorkerAbsenceDTO.AbsenceStartDate);
                if (!entityWorkerAbsence.IsFullDay)
                {
                    entityWorkerAbsence.AbsenceStartDate = entityWorkerAbsence.AbsenceStartDate.ToUniversalTime();
                    entityWorkerAbsence.AbsenceEndDate = entityWorkerAbsence.AbsenceEndDate.ToUniversalTime();
                }


                // if there is an previous approval decision, reset it
                if (entityWorkerAbsence.AbsenceDecisionOwner != null && entityWorkerAbsence.AbsenceDateDecision != new DateTime())
                {
                    entityWorkerAbsence.AbsenceDecisionOwner = string.Empty;
                    entityWorkerAbsence.AbsenceDateDecision = new DateTime();
                    entityWorkerAbsence.AbsenceDateDecisionOffset = 0;
                    entityWorkerAbsence.AbsenceApproved = false;
                }

                try
                {
                    await _unitOfWork.EntityWorkerAbsenceRepository.Update(entityWorkerAbsence);

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

        #region AUX - Get Time Zone Offset Minutes

        public static int GetTimeZoneOffsetMinutes(string timeZoneId, DateTime? dateTime = null)
        {
            if (string.IsNullOrWhiteSpace(timeZoneId))
                throw new ArgumentException("Time zone ID cannot be null or empty.", nameof(timeZoneId));

            // Default to now if no datetime is provided
            DateTime targetDate = dateTime ?? DateTime.UtcNow;

            TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

            // Get the offset considering DST
            TimeSpan offset = tz.GetUtcOffset(targetDate);

            return (int)offset.TotalMinutes;
        }

        #endregion

        #endregion
    }
}
