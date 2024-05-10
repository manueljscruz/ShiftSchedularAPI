using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.ShiftManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShiftSchedularBLL.Service
{
    public class ShiftService : IShiftService
    {
        private readonly IMapper _mapper;
        private readonly IGeneralService _generalService;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEntityRepository _entityRepository;
        private readonly IShiftRepository _shiftRepository;
        private readonly IShiftBreakRepository _shiftBreakRepository;
        private readonly IGenericRepository<ShiftBreakType> _shiftBreakTypeRepository;

        #region Constructor

        public ShiftService(IMapper mapper, 
            IGeneralService generalService, 
            IUnitOfWork unitOfWork, 
            IEntityRepository entityRepository, 
            IShiftRepository shiftRepository, 
            IShiftBreakRepository shiftBreakRepository,
            IGenericRepository<ShiftBreakType> shiftBreakTypeRepository) 
        {
            _mapper = mapper;
            _generalService = generalService;
            _unitOfWork = unitOfWork;
            _entityRepository = entityRepository;
            _shiftRepository = shiftRepository;
            _shiftBreakRepository = shiftBreakRepository;
            _shiftBreakTypeRepository = shiftBreakTypeRepository;
        }


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

            if(addShiftDTO != null)
            {
                // No destination entity
                if (string.IsNullOrEmpty(addShiftDTO.EntityId))
                {
                    response.Message = ShiftRelatedMessages.ShiftEntityIdIsNull;
                    return response;
                }

                // Name is Empty
                else if(string.IsNullOrEmpty(addShiftDTO.ShiftName)) 
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftNameEmpty;
                    return response;
                }

                // No shift duration
                else if(addShiftDTO.ShiftDuration == TimeSpan.Zero)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftDurationIsNull;
                    return response;
                }

                // Get entity and check ifs null
                Entity destinationEntity = await _entityRepository.GetById(addShiftDTO.EntityId);
                if(destinationEntity == null)
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
                            foreach(AddShiftBreakDTO addShiftBreakDTO in addShiftDTO.ShiftBreakDTOs)
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
                        await _unitOfWork.SaveChangesAsync();
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

                else if(addShiftBreakDTO.ShiftBreakTypeId == 0)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftBreakEmptyShift;
                    return response;
                }

                ShiftBreakType shiftBreakType = await _shiftBreakTypeRepository.GetById(addShiftBreakDTO.ShiftBreakTypeId);

                if(shiftBreakType == null)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftBreakBreakTypeNotFound;
                    return response;
                }

                else if(addShiftBreakDTO.ShiftBreakDuration == TimeSpan.Zero)
                {
                    response.Message = ShiftRelatedMessages.AddNewShiftBreakDurationIsNull;
                    return response;
                }

                ShiftBreak shiftBreak = _mapper.Map<ShiftBreak>(addShiftBreakDTO);
                shiftBreak.ShiftId = _generalService.GenerateGuid();

                shiftBreak = await _shiftBreakRepository.Add(shiftBreak);
                response.Result = _mapper.Map<ShiftBreakDTO>(shiftBreak);
            }

            return response;
        }

        #endregion

        #region Delete Entity Shift

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
            else if(string.IsNullOrEmpty(shiftId))
            {
                response.Message = ShiftRelatedMessages.ShiftIdIsNull;
                return response;
            }

            Shift shiftInstance = await _shiftRepository.GetById(shiftId);
            if(shiftInstance == null)
            {
                response.Message = ShiftRelatedMessages.ShiftNotFound;
                return response;
            }

            IEnumerable<ShiftBreak> shiftBreaks = await _shiftBreakRepository.GetBreaksByShiftId(shiftId);
            if(shiftBreaks == null)
            {
                response.Message = ShiftRelatedMessages.DeleteShiftBreaksNotFound;
                return response;
            }

            try
            {
                await _unitOfWork.BeginTransactionAsync();

                // If there are shift breaks related, remove them
                if(shiftBreaks.Count() != 0)
                    await _shiftBreakRepository.DeleteRange(shiftBreaks);

                await _shiftRepository.Delete(shiftInstance.ShiftId);

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

        public async Task<BaseResponse<bool>> DeleteEntityShiftBreak(DeleteEntityShiftBreakDTO deleteEntityShiftBreak)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;
            response.Message = "";

            if(deleteEntityShiftBreak != null)
            {
                if (string.IsNullOrEmpty(deleteEntityShiftBreak.ShiftBreakId))
                {
                    response.Message = ShiftRelatedMessages.DeleteShiftBreakIdIsNull;
                    return response;
                }

                ShiftBreak shiftBreak = await _shiftBreakRepository.GetById(deleteEntityShiftBreak.ShiftBreakId);
                if(shiftBreak == null)
                {
                    response.Message = "";
                    return response;
                }

                await _shiftBreakRepository.Delete(shiftBreak.ShiftBreakId);

                response.Success = true;
                response.Message = "";

            }

            return response;
        }

        public Task<IEnumerable<ShiftDTO>> GetAllEntityShifts(string entityId, string lcode)
        {
            throw new NotImplementedException();
        }

        public Task<ShiftDTO> GetShiftById(string id)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<bool>> UpdateEntityShift(ShiftDTO shift)
        {
            throw new NotImplementedException();
        }

        public Task<BaseResponse<bool>> UpdateEntityShiftBreak(ShiftBreakDTO shiftBreak)
        {
            throw new NotImplementedException();
        }

        #endregion



        #endregion

    }
}
