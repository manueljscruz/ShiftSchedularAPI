using AutoMapper;
using Microsoft.IdentityModel.Tokens;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.Repositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularEntity.Models.ViewModels;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.ShiftManagement;

namespace ShiftSchedularBLL.Service
{
    public class ShiftService : IShiftService
    {
        private readonly IMapper _mapper;
        private readonly IGeneralService _generalService;
        private readonly IShiftTemplateService _shiftTemplateService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<Entity> _entityRepository;
        private readonly IShiftRepository _shiftRepository;
        private readonly IShiftBreakRepository _shiftBreakRepository;
        private readonly IGenericRepository<ShiftBreakType> _shiftBreakTypeRepository;
        private readonly IEntityWorkerRepository _entityWorkerRepository;
        private readonly IShiftBreakTypeLocalizationRepository _shiftBreakTypeLocalizationRepository;
        private readonly IGenericRepository<ShiftBreakTemplate> _shiftBreakTemplateRepository;
        private readonly IGenericRepository<ShiftTemplate> _shiftTemplateRepository;


        #region Constructor

        public ShiftService(IMapper mapper,
            IGeneralService generalService,
            IShiftTemplateService shiftTemplateService,
            IUnitOfWork unitOfWork,
            IGenericRepository<Entity> entityRepository,
            IShiftRepository shiftRepository,
            IShiftBreakRepository shiftBreakRepository,
            IEntityWorkerRepository entityWorkerRepository,
            IGenericRepository<ShiftBreakType> shiftBreakTypeRepository,
            IGenericRepository<ShiftBreakTemplate> shiftBreakTemplateRepository,
            IShiftBreakTypeLocalizationRepository shiftBreakTypeLocalizationRepository,
            IGenericRepository<ShiftTemplate> shiftTemplateRepository)
        {
            _mapper = mapper;
            _generalService = generalService;
            _shiftTemplateService = shiftTemplateService;
            _unitOfWork = unitOfWork;
            _entityRepository = entityRepository;
            _shiftRepository = shiftRepository;
            _shiftBreakRepository = shiftBreakRepository;
            _entityWorkerRepository = entityWorkerRepository;
            _shiftBreakTypeRepository = shiftBreakTypeRepository;
            _shiftBreakTemplateRepository = shiftBreakTemplateRepository;
            _shiftBreakTypeLocalizationRepository = shiftBreakTypeLocalizationRepository;
            _shiftTemplateRepository = shiftTemplateRepository;
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
                if (string.IsNullOrEmpty(addShiftDTO.EntityId))
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
                Entity destinationEntity = await _entityRepository.GetById(addShiftDTO.EntityId);
                if (destinationEntity == null)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftEntityNotFound;
                    return response;
                }
                else
                {
                    try
                    {
                        await _unitOfWork.BeginTransactionAsync();

                        // Map shift, create Id and add it
                        Shift newShift = _mapper.Map<Shift>(addShiftDTO);
                        newShift.ShiftId = _generalService.GenerateGuid();
                        newShift = await _shiftRepository.Add(newShift);

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

                        response.Result = shiftDTO;
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
                if (string.IsNullOrEmpty(addShiftBreakDTO.ShiftId))
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftBreakEmptyShift;
                    return response;
                }

                else if (addShiftBreakDTO.ShiftBreakTypeId == 0)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftBreakEmptyShift;
                    return response;
                }

                ShiftBreakType shiftBreakType = await _shiftBreakTypeRepository.GetById(addShiftBreakDTO.ShiftBreakTypeId);

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
                shiftBreak.ShiftBreakId = _generalService.GenerateGuid();

                shiftBreak = await _shiftBreakRepository.Add(shiftBreak);
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
        public async Task<BaseResponse<bool>> DeleteEntityShift(string entityId, string shiftId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = "";

            if (string.IsNullOrEmpty(entityId))
            {
                response.Message = ShiftRelatedMessages.ShiftEntityIdIsNull;
                return response;
            }
            else if (string.IsNullOrEmpty(shiftId))
            {
                response.Message = ShiftRelatedMessages.ShiftIdIsNull;
                return response;
            }

            Shift shiftInstance = await _shiftRepository.GetById(shiftId);
            if (shiftInstance == null)
            {
                response.Message = ShiftRelatedMessages.ShiftNotFound;
                return response;
            }

            IEnumerable<ShiftBreak> shiftBreaks = await _shiftBreakRepository.GetBreaksByShiftId(shiftId);
            if (shiftBreaks == null)
            {
                response.Message = ShiftRelatedMessages.DeleteShiftBreaksNotFound;
                return response;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // If there are shift breaks related, remove them
                if (shiftBreaks.Count() != 0)
                    await _shiftBreakRepository.DeleteRange(shiftBreaks);

                await _shiftRepository.Delete(shiftInstance.ShiftId);

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
        public async Task<BaseResponse<bool>> DeleteEntityShiftBreak(string shiftBreakId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = ShiftRelatedMessages.DeleteShiftBreakUnexpectedError;

            if (string.IsNullOrEmpty(shiftBreakId))
            {
                response.Message = ShiftRelatedMessages.DeleteShiftBreakIdIsNull;
                return response;
            }

            ShiftBreak shiftBreak = await _shiftBreakRepository.GetById(shiftBreakId);
            if (shiftBreak == null)
            {
                response.Message = ShiftRelatedMessages.DeleteShiftBreaksNotFound;
                return response;
            }

            await _shiftBreakRepository.Delete(shiftBreak.ShiftBreakId);

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
        public async Task<ShiftViewModel> GetEntityShiftsViewModel(EntityShiftViewModelRequestDTO shiftViewModelRequestDTO)
        {
            ShiftViewModel shiftViewModel = new ShiftViewModel();

            // If necessary data is different than empty
            if (shiftViewModelRequestDTO != null && !string.IsNullOrEmpty(shiftViewModelRequestDTO.EntityId) && !string.IsNullOrEmpty(shiftViewModelRequestDTO.WorkerId) && !string.IsNullOrEmpty(shiftViewModelRequestDTO.LanguageCode))
            {
                Entity entity = await _entityRepository.GetById(shiftViewModelRequestDTO.EntityId);
                EntityWorker entityWorker = await _entityWorkerRepository.GetByWorkerAndEntity(shiftViewModelRequestDTO.WorkerId, shiftViewModelRequestDTO.EntityId);
                shiftViewModel.AllowEdit = entityWorker.IsOwner;

                // If it can change data
                if (entityWorker.IsOwner)
                {
                    // Get Shift Break Types Localized
                    IEnumerable<ShiftBreakTypeLocalization> shiftBreakTypeLocalizations = await _shiftBreakTypeLocalizationRepository.GetShiftBreaksTypeLocalized(shiftViewModelRequestDTO.LanguageCode);
                    foreach (ShiftBreakTypeLocalization shiftBreakTypeLocalization in shiftBreakTypeLocalizations)
                        shiftViewModel.ShiftBreakTypeLocalizeds.Add(_mapper.Map<ShiftBreakTypeLocalization, ShiftBreakTypeLocalizedDTO>(shiftBreakTypeLocalization));

                    // Get Shift Breaks Templates
                    shiftViewModel.ShiftBreakTemplates = await _shiftTemplateService.GetShiftBreakTemplates(shiftViewModelRequestDTO.LanguageCode);

                    shiftViewModel.ShiftTemplates = await _shiftTemplateService.GetShiftTemplates(shiftViewModelRequestDTO.LanguageCode);

                    // Get Shifts
                    IEnumerable<Shift> shifts = await _shiftRepository.GetEntityShifts(shiftViewModelRequestDTO.EntityId);
                    foreach (Shift shift in shifts)
                    {
                        ShiftDTO shiftDTO = await HandleShiftData(shift, shiftBreakTypeLocalizations);
                        shiftViewModel.Shifts.Add(shiftDTO);

                    }
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
        /// <param name="lcode"></param>
        /// <returns></returns>
        public async Task<ShiftDTO> GetShiftById(string shiftId, string lcode)
        {
            ShiftDTO shiftDTO = new ShiftDTO();

            // If there is a shift id 
            if (!string.IsNullOrEmpty(shiftId) && !string.IsNullOrEmpty(lcode))
            {
                // Get shift and proceed if its different than null
                Shift shift = await _shiftRepository.GetById(shiftId);
                if (shift != null)
                {
                    IEnumerable<ShiftBreakTypeLocalization> shiftBreakTypeLocalizations = await _shiftBreakTypeLocalizationRepository.GetShiftBreaksTypeLocalized(lcode);
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
        public async Task<IEnumerable<ShiftDTO>> GetEntityShifts(string entityId)
        {
            List<ShiftDTO> shiftsDTO = new List<ShiftDTO>();

            if (!string.IsNullOrEmpty(entityId))
            {
                Entity entity = await _entityRepository.GetById(entityId);
                if(entity != null)
                {
                    IEnumerable<Shift> shifts = await _shiftRepository.GetEntityShifts(entityId);
                    foreach(Shift shift in shifts)
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

            // Get Shift breaks related to the shift
            IEnumerable<ShiftBreak> shiftBreaks = await _shiftBreakRepository.GetBreaksByShiftId(shift.ShiftId);

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
                Shift shiftInstance = await _shiftRepository.GetById(shift.ShiftId);
                if (shiftInstance == null)
                {
                    response.Message = ShiftRelatedMessages.UpdateShiftNotFound;
                    return response;
                }

                // No destination entity
                else if (string.IsNullOrEmpty(shift.EntityId))
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
                Entity destinationEntity = await _entityRepository.GetById(shift.EntityId);
                if (destinationEntity == null)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftEntityNotFound;
                    return response;
                }

                _mapper.Map(shift, shiftInstance);
                await _shiftRepository.Update(shiftInstance);
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
                ShiftBreak shiftBreakInstance = await _shiftBreakRepository.GetById(shiftBreak.ShiftParentId);
                if (shiftBreak == null)
                {
                    response.Message = ShiftRelatedMessages.DeleteShiftBreaksNotFound;
                    return response;
                }

                else if (string.IsNullOrEmpty(shiftBreak.ShiftParentId))
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftBreakEmptyShift;
                    return response;
                }

                else if (shiftBreak.ShiftBreakTypeId == 0)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftBreakEmptyShift;
                    return response;
                }

                ShiftBreakType shiftBreakType = await _shiftBreakTypeRepository.GetById(shiftBreak.ShiftBreakTypeId);

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
                await _shiftBreakRepository.Update(shiftBreakInstance);
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

            if(shiftIdentifiers.Count != 0)
            {
                List<Shift> shifts = (List<Shift>) await _shiftRepository.GetEntityShifts(shiftIdentifiers);

                foreach (Shift shift in shifts)
                {
                    ShiftDTO shiftDTO = _mapper.Map<ShiftDTO>(shift);
                    shiftDTOs.Add(shiftDTO);
                }
            }

            return shiftDTOs;
        }


        #endregion

        #endregion
    }
}
