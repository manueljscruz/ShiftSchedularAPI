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
using ShiftSchedularRL.Resources.Holidays;

namespace ShiftSchedularBLL.Service
{
    /// <summary>
    /// Service implementation for Holiday management operations.
    /// Handles CRUD operations for HolidayType, HolidayBehaviour, HolidayCatalog, and EntityHoliday entities.
    /// </summary>
    public class HolidayService : IHolidayService
    {
        #region Fields

        private readonly IUnitOfWork _unitOfWork;
        private readonly IGeneralService _generalService;
        private readonly IMapper _mapper;
        private readonly IGenericRepository<HolidayType> _holidayTypeRepository;
        private readonly IGenericRepository<HolidayBehaviour> _holidayBehaviourRepository;
        private readonly IGenericRepository<HolidayCatalog> _holidayCatalogRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the HolidayService.
        /// </summary>
        /// <param name="unitOfWork">The unit of work for transaction management and repository access</param>
        /// <param name="generalService">General utility service for GUID operations</param>
        /// <param name="mapper">AutoMapper instance for object mapping</param>
        /// <param name="holidayTypeRepository">Generic repository for HolidayType operations</param>
        /// <param name="holidayBehaviourRepository">Generic repository for HolidayBehaviour operations</param>
        /// <param name="holidayCatalogRepository">Generic repository for HolidayCatalog operations</param>
        public HolidayService(
            IUnitOfWork unitOfWork,
            IGeneralService generalService,
            IMapper mapper,
            IGenericRepository<HolidayType> holidayTypeRepository,
            IGenericRepository<HolidayBehaviour> holidayBehaviourRepository,
            IGenericRepository<HolidayCatalog> holidayCatalogRepository)
        {
            _unitOfWork = unitOfWork;
            _generalService = generalService;
            _mapper = mapper;
            _holidayTypeRepository = holidayTypeRepository;
            _holidayBehaviourRepository = holidayBehaviourRepository;
            _holidayCatalogRepository = holidayCatalogRepository;
        }

        #endregion

        #region HolidayType Methods

        /// <summary>
        /// Adds a new holiday type to the system.
        /// </summary>
        /// <param name="strNewHolidayType">The name for the new holiday type</param>
        /// <returns>The ID of the newly created holiday type, or 0 if creation failed</returns>
        public async Task<int> AddHolidayType(string strNewHolidayType)
        {
            if (string.IsNullOrWhiteSpace(strNewHolidayType))
                return 0;

            HolidayType newHolidayType = new HolidayType
            {
                HolidayTypeName = strNewHolidayType.Trim()
            };

            newHolidayType = await _holidayTypeRepository.Add(newHolidayType);

            return newHolidayType.HolidayTypeId;
        }

        /// <summary>
        /// Retrieves a holiday type by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the holiday type</param>
        /// <returns>The HolidayType entity if found, otherwise null</returns>
        public async Task<HolidayType> GetHolidayTypeById(int id)
        {
            if (id <= 0)
                return null;

            return await _holidayTypeRepository.GetById(id);
        }

        /// <summary>
        /// Updates an existing holiday type.
        /// </summary>
        /// <param name="holidayType">The holiday type entity with updated values</param>
        /// <returns>A BaseResponse indicating success or failure</returns>
        public async Task<BaseResponse<bool>> UpdateHolidayType(HolidayType holidayType)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            if (holidayType == null || holidayType.HolidayTypeId <= 0)
            {
                response.Message = HolidayRelatedMessages.HolidayTypeInvalidData;
                return response;
            }

            try
            {
                HolidayType existingType = await _holidayTypeRepository.GetById(holidayType.HolidayTypeId);

                if (existingType == null)
                {
                    response.Message = HolidayRelatedMessages.HolidayTypeNotFound;
                    return response;
                }

                existingType.HolidayTypeName = holidayType.HolidayTypeName;
                await _holidayTypeRepository.Update(existingType);

                response.Success = true;
                response.Result = true;
                response.Message = HolidayRelatedMessages.HolidayTypeUpdatedSuccess;
            }
            catch (Exception)
            {
                response.Message = HolidayRelatedMessages.HolidayTypeUpdateError;
            }

            return response;
        }

        /// <summary>
        /// Deletes a holiday type by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the holiday type to delete</param>
        /// <returns>A BaseResponse indicating success or failure</returns>
        public async Task<BaseResponse<bool>> DeleteHolidayType(int id)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            if (id <= 0)
            {
                response.Message = HolidayRelatedMessages.HolidayTypeInvalidId;
                return response;
            }

            try
            {
                await _holidayTypeRepository.Delete(id);

                response.Success = true;
                response.Result = true;
                response.Message = HolidayRelatedMessages.HolidayTypeDeletedSuccess;
            }
            catch (Exception)
            {
                response.Message = HolidayRelatedMessages.HolidayTypeDeleteError;
            }

            return response;
        }

        #endregion

        #region HolidayBehaviour Methods

        /// <summary>
        /// Adds a new holiday behaviour to the system.
        /// </summary>
        /// <param name="strNewHolidayBehaviour">The name for the new holiday behaviour</param>
        /// <returns>The ID of the newly created holiday behaviour, or 0 if creation failed</returns>
        public async Task<int> AddHolidayBehaviour(string strNewHolidayBehaviour)
        {
            if (string.IsNullOrWhiteSpace(strNewHolidayBehaviour))
                return 0;

            HolidayBehaviour newHolidayBehaviour = new HolidayBehaviour
            {
                HolidayBehaviourName = strNewHolidayBehaviour.Trim()
            };

            newHolidayBehaviour = await _holidayBehaviourRepository.Add(newHolidayBehaviour);

            return newHolidayBehaviour.HolidayBehaviourId;
        }

        /// <summary>
        /// Retrieves a holiday behaviour by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the holiday behaviour</param>
        /// <returns>The HolidayBehaviour entity if found, otherwise null</returns>
        public async Task<HolidayBehaviour> GetHolidayBehaviourById(int id)
        {
            if (id <= 0)
                return null;

            return await _holidayBehaviourRepository.GetById(id);
        }

        /// <summary>
        /// Updates an existing holiday behaviour.
        /// </summary>
        /// <param name="holidayBehaviour">The holiday behaviour entity with updated values</param>
        /// <returns>A BaseResponse indicating success or failure</returns>
        public async Task<BaseResponse<bool>> UpdateHolidayBehaviour(HolidayBehaviour holidayBehaviour)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            if (holidayBehaviour == null || holidayBehaviour.HolidayBehaviourId <= 0)
            {
                response.Message = HolidayRelatedMessages.HolidayBehaviourInvalidData;
                return response;
            }

            try
            {
                HolidayBehaviour existingBehaviour = await _holidayBehaviourRepository.GetById(holidayBehaviour.HolidayBehaviourId);

                if (existingBehaviour == null)
                {
                    response.Message = HolidayRelatedMessages.HolidayBehaviourNotFound;
                    return response;
                }

                existingBehaviour.HolidayBehaviourName = holidayBehaviour.HolidayBehaviourName;
                await _holidayBehaviourRepository.Update(existingBehaviour);

                response.Success = true;
                response.Result = true;
                response.Message = HolidayRelatedMessages.HolidayBehaviourUpdatedSuccess;
            }
            catch (Exception)
            {
                response.Message = HolidayRelatedMessages.HolidayBehaviourUpdateError;
            }

            return response;
        }

        /// <summary>
        /// Deletes a holiday behaviour by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the holiday behaviour to delete</param>
        /// <returns>A BaseResponse indicating success or failure</returns>
        public async Task<BaseResponse<bool>> DeleteHolidayBehaviour(int id)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            if (id <= 0)
            {
                response.Message = HolidayRelatedMessages.HolidayBehaviourInvalidId;
                return response;
            }

            try
            {
                await _holidayBehaviourRepository.Delete(id);

                response.Success = true;
                response.Result = true;
                response.Message = HolidayRelatedMessages.HolidayBehaviourDeletedSuccess;
            }
            catch (Exception)
            {
                response.Message = HolidayRelatedMessages.HolidayBehaviourDeleteError;
            }

            return response;
        }

        #endregion

        #region HolidayCatalog Methods

        /// <summary>
        /// Adds a new holiday catalog entry to the system.
        /// </summary>
        /// <param name="addHolidayCatalogDTO">The DTO containing the new holiday catalog data</param>
        /// <returns>The ID of the newly created holiday catalog entry, or 0 if creation failed</returns>
        public async Task<int> AddHolidayCatalog(AddHolidayCatalogDTO addHolidayCatalogDTO)
        {
            if (addHolidayCatalogDTO == null || string.IsNullOrWhiteSpace(addHolidayCatalogDTO.HolidayName))
                return 0;

            HolidayCatalog newCatalog = _mapper.Map<HolidayCatalog>(addHolidayCatalogDTO);
            newCatalog = await _holidayCatalogRepository.Add(newCatalog);

            return newCatalog.HolidayCatalogId;
        }

        /// <summary>
        /// Retrieves a holiday catalog entry by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the holiday catalog entry</param>
        /// <returns>The HolidayCatalog entity if found, otherwise null</returns>
        public async Task<HolidayCatalog> GetHolidayCatalogById(int id)
        {
            if (id <= 0)
                return null;

            return await _holidayCatalogRepository.GetById(id);
        }

        /// <summary>
        /// Updates an existing holiday catalog entry.
        /// </summary>
        /// <param name="holidayCatalog">The holiday catalog entity with updated values</param>
        /// <returns>A BaseResponse indicating success or failure</returns>
        public async Task<BaseResponse<bool>> UpdateHolidayCatalog(HolidayCatalog holidayCatalog)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            if (holidayCatalog == null || holidayCatalog.HolidayCatalogId <= 0)
            {
                response.Message = HolidayRelatedMessages.HolidayCatalogInvalidData;
                return response;
            }

            try
            {
                HolidayCatalog existingCatalog = await _holidayCatalogRepository.GetById(holidayCatalog.HolidayCatalogId);

                if (existingCatalog == null)
                {
                    response.Message = HolidayRelatedMessages.HolidayCatalogNotFound;
                    return response;
                }

                // Update all editable fields
                existingCatalog.HolidayTypeId = holidayCatalog.HolidayTypeId;
                existingCatalog.HolidayBehaviourId = holidayCatalog.HolidayBehaviourId;
                existingCatalog.HolidayName = holidayCatalog.HolidayName;
                existingCatalog.HolidayDescription = holidayCatalog.HolidayDescription;
                existingCatalog.RecurrenceDay = holidayCatalog.RecurrenceDay;
                existingCatalog.RecurrenceMonth = holidayCatalog.RecurrenceMonth;
                existingCatalog.IsRecurring = holidayCatalog.IsRecurring;
                existingCatalog.IsActive = holidayCatalog.IsActive;

                await _holidayCatalogRepository.Update(existingCatalog);

                response.Success = true;
                response.Result = true;
                response.Message = HolidayRelatedMessages.HolidayCatalogUpdatedSuccess;
            }
            catch (Exception)
            {
                response.Message = HolidayRelatedMessages.HolidayCatalogUpdateError;
            }

            return response;
        }

        /// <summary>
        /// Deletes a holiday catalog entry by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the holiday catalog entry to delete</param>
        /// <returns>A BaseResponse indicating success or failure</returns>
        public async Task<BaseResponse<bool>> DeleteHolidayCatalog(int id)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            if (id <= 0)
            {
                response.Message = HolidayRelatedMessages.HolidayCatalogInvalidId;
                return response;
            }

            try
            {
                await _holidayCatalogRepository.Delete(id);

                response.Success = true;
                response.Result = true;
                response.Message = HolidayRelatedMessages.HolidayCatalogDeletedSuccess;
            }
            catch (Exception)
            {
                response.Message = HolidayRelatedMessages.HolidayCatalogDeleteError;
            }

            return response;
        }

        #endregion

        #region EntityHoliday Methods

        /// <summary>
        /// Adds a new entity holiday (custom or from catalog) to the system.
        /// </summary>
        /// <param name="addEntityHolidayDTO">The DTO containing the entity holiday data</param>
        /// <returns>A BaseResponse containing the created EntityHolidayDTO</returns>
        public async Task<BaseResponse<EntityHolidayDTO>> AddEntityHoliday(AddEntityHolidayDTO addEntityHolidayDTO)
        {
            BaseResponse<EntityHolidayDTO> response = new BaseResponse<EntityHolidayDTO>();

            if (addEntityHolidayDTO == null || addEntityHolidayDTO.EntityId == Guid.Empty)
            {
                response.Message = HolidayRelatedMessages.EntityHolidayInvalidData;
                return response;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // Map DTO to entity
                EntityHoliday entityHoliday = _mapper.Map<EntityHoliday>(addEntityHolidayDTO);
                entityHoliday.EntityHolidayId = Guid.NewGuid();

                // Add to repository
                await _unitOfWork.EntityHolidayRepository.Add(entityHoliday);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                // Retrieve the created entity with navigation properties
                EntityHoliday createdHoliday = await _unitOfWork.EntityHolidayRepository.GetEntityHolidayById(entityHoliday.EntityHolidayId);
                EntityHolidayDTO resultDTO = _mapper.Map<EntityHolidayDTO>(createdHoliday);

                response.Success = true;
                response.Result = resultDTO;
                response.Message = HolidayRelatedMessages.EntityHolidayCreatedSuccess;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = HolidayRelatedMessages.EntityHolidayCreateError;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return response;
        }

        /// <summary>
        /// Retrieves an entity holiday by its unique identifier.
        /// </summary>
        /// <param name="id">The unique identifier of the entity holiday</param>
        /// <returns>The EntityHolidayDTO if found, otherwise null</returns>
        public async Task<EntityHolidayDTO> GetEntityHolidayById(Guid id)
        {
            if (id == Guid.Empty)
                return null;

            EntityHoliday entityHoliday = await _unitOfWork.EntityHolidayRepository.GetEntityHolidayById(id);

            if (entityHoliday == null)
                return null;

            return _mapper.Map<EntityHolidayDTO>(entityHoliday);
        }

        /// <summary>
        /// Retrieves a paginated list of entity holidays for a specific entity.
        /// </summary>
        /// <param name="entityHolidaysPaginationRequest">The pagination request containing entity ID and page parameters</param>
        /// <returns>A paginated list of EntityHolidayDTO records</returns>
        public async Task<PagedList<EntityHolidayDTO>> GetEntityHolidaysPagination(PagedModelRequest entityHolidaysPaginationRequest)
        {
            if (entityHolidaysPaginationRequest == null || entityHolidaysPaginationRequest.EntityId == Guid.Empty)
                return PagedList<EntityHolidayDTO>.CreateEmpty();

            // Get paginated entity holidays
            PagedList<EntityHoliday> pagedHolidays = await _unitOfWork.EntityHolidayRepository.GetEntityHolidaysPaginated(
                entityHolidaysPaginationRequest.EntityId,
                entityHolidaysPaginationRequest.NextPage,
                entityHolidaysPaginationRequest.ItemsPerPage);

            // Map the Data list to DTOs
            List<EntityHolidayDTO> holidayDTOs = _mapper.Map<List<EntityHolidayDTO>>(pagedHolidays.Data);

            return PagedList<EntityHolidayDTO>.Create(
                holidayDTOs.AsQueryable(),
                pagedHolidays.TotalCount,
                pagedHolidays.CurrentPage,
                pagedHolidays.PageSize);
        }

        /// <summary>
        /// Updates an existing entity holiday.
        /// </summary>
        /// <param name="entityHolidayDTO">The entity holiday DTO with updated values</param>
        /// <returns>A BaseResponse indicating success or failure</returns>
        public async Task<BaseResponse<bool>> UpdateEntityHoliday(EntityHolidayDTO entityHolidayDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            if (entityHolidayDTO == null || entityHolidayDTO.EntityHolidayId == Guid.Empty)
            {
                response.Message = HolidayRelatedMessages.EntityHolidayInvalidData;
                return response;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                EntityHoliday existingHoliday = await _unitOfWork.EntityHolidayRepository.GetEntityHolidayById(entityHolidayDTO.EntityHolidayId);

                if (existingHoliday == null)
                {
                    response.Message = HolidayRelatedMessages.EntityHolidayNotFound;
                    return response;
                }

                // Update fields
                existingHoliday.CustomHolidayName = entityHolidayDTO.CustomHolidayName;
                existingHoliday.CustomDay = entityHolidayDTO.CustomDay;
                existingHoliday.CustomMonth = entityHolidayDTO.CustomMonth;
                existingHoliday.OperatingStartTime = entityHolidayDTO.OperatingStartTime;
                existingHoliday.OperatingEndTime = entityHolidayDTO.OperatingEndTime;
                existingHoliday.IsActive = entityHolidayDTO.IsActive;
                existingHoliday.Notes = entityHolidayDTO.Notes;

                await _unitOfWork.EntityHolidayRepository.Update(existingHoliday);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Result = true;
                response.Message = HolidayRelatedMessages.EntityHolidayUpdatedSuccess;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = HolidayRelatedMessages.EntityHolidayUpdateError;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return response;
        }

        /// <summary>
        /// Deletes an entity holiday.
        /// </summary>
        /// <param name="entityHolidayDTO">The entity holiday DTO containing the ID to delete</param>
        /// <returns>A BaseResponse indicating success or failure</returns>
        public async Task<BaseResponse<bool>> DeleteEntityHoliday(EntityHolidayDTO entityHolidayDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            if (entityHolidayDTO == null || entityHolidayDTO.EntityHolidayId == Guid.Empty)
            {
                response.Message = HolidayRelatedMessages.EntityHolidayInvalidData;
                return response;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                await _unitOfWork.EntityHolidayRepository.Delete(entityHolidayDTO.EntityHolidayId);
                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();

                response.Success = true;
                response.Result = true;
                response.Message = HolidayRelatedMessages.EntityHolidayDeletedSuccess;
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                response.Message = HolidayRelatedMessages.EntityHolidayDeleteError;
            }
            finally
            {
                _unitOfWork.Dispose();
            }

            return response;
        }

        #endregion

        #region Get Entity Holidays View Model

        /// <summary>
        /// Retrieves the complete view model for entity holidays management.
        /// Includes holiday types, behaviours, catalog entries, and entity-specific holidays all localized.
        /// </summary>
        /// <param name="viewModelRequestDTO">The view model request containing entity ID and language code</param>
        /// <returns>An EntityHolidaysViewModel populated with all required data</returns>
        public async Task<EntityHolidaysViewModel> GetEntityHolidaysViewModel(BaseViewModelRequest viewModelRequestDTO)
        {
            EntityHolidaysViewModel viewModel = new EntityHolidaysViewModel
            {
                IsOwner = false,
                HolidayCatalogDTOs = new List<HolidayCatalogLocalizedDTO>(),
                HolidayBehaviourDTOs = new List<HolidayBehaviourLocalizedDTO>(),
                HolidayTypeDTOs = new List<HolidayTypeLocalizedDTO>(),
                EntityHolidayDTOs = new List<EntityHolidayDTO>()
            };

            if (viewModelRequestDTO == null || viewModelRequestDTO.EntityId == Guid.Empty)
                return viewModel;

            // Extract language code (handle format like "en-US" -> "en")
            string languageCode = viewModelRequestDTO.LanguageCode;
            if (!string.IsNullOrEmpty(languageCode) && languageCode.Contains("-"))
                languageCode = languageCode.Split('-')[0];

            try
            {
                // Determine if the worker is the owner of the entity
                if (!string.IsNullOrEmpty(viewModelRequestDTO.WorkerId))
                {
                    viewModel.IsOwner = await _unitOfWork.EntityWorkerRepository.IsMemberOwner(
                        viewModelRequestDTO.EntityId,
                        viewModelRequestDTO.WorkerId);
                }

                // Get Holiday Types Localized
                IEnumerable<HolidayTypeLocalization> holidayTypeLocalizations =
                    await _unitOfWork.HolidayTypeLocalizationRepository.GetHolidayTypesByLocalization(languageCode);
                viewModel.HolidayTypeDTOs = _mapper.Map<List<HolidayTypeLocalizedDTO>>(holidayTypeLocalizations);

                // Get Holiday Behaviours Localized
                IEnumerable<HolidayBehaviourLocalization> holidayBehaviourLocalizations =
                    await _unitOfWork.HolidayBehaviourLocalizationRepository.GetHolidayBehavioursByLocalization(languageCode);
                viewModel.HolidayBehaviourDTOs = _mapper.Map<List<HolidayBehaviourLocalizedDTO>>(holidayBehaviourLocalizations);

                // Get Holiday Catalog Localized
                IEnumerable<HolidayCatalogLocalization> holidayCatalogLocalizations =
                    await _unitOfWork.HolidayCatalogLocalizationRepository.GetHolidayCatalogsByLocalization(languageCode);
                viewModel.HolidayCatalogDTOs = _mapper.Map<List<HolidayCatalogLocalizedDTO>>(holidayCatalogLocalizations);

                // Get Entity Holidays
                IEnumerable<EntityHoliday> entityHolidays =
                    await _unitOfWork.EntityHolidayRepository.GetEntityHolidays(viewModelRequestDTO.EntityId);
                viewModel.EntityHolidayDTOs = _mapper.Map<List<EntityHolidayDTO>>(entityHolidays);
            }
            catch (Exception)
            {
                // Return empty view model on error
                // Consider logging the exception
            }

            return viewModel;
        }

        #endregion
    }
}
