using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.Home;

namespace ShiftSchedularBLL.Service
{
    public class WorkerService : IWorkerService
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly IGeneralService _generalService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        public WorkerService(ICryptographyService cryptographyService, IGeneralService generalService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _cryptographyService = cryptographyService;
            _generalService = generalService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        #region Create Worker

        /// <summary>
        /// Creates a Worker in the database
        /// </summary>
        /// <param name="newWorker"></param>
        /// <returns></returns>
        public async Task<BaseResponse<bool>> CreateWorker(NewWorkerDTO newWorker)
        {
            BaseResponse<bool> result = new BaseResponse<bool>();
            result.Result = false;

            try 
            { 
                // Checks if there is already an email in use
                if(_unitOfWork.WorkerRepository.GetByEmail(newWorker.Email) != null)
                {
                    result.Message = WorkerRelatedMessages.WorkerEmailInUseError;
                    return result;
                }

                Worker worker = _mapper.Map<Worker>(newWorker);
                worker.IsActive = true;
                worker.WorkerId = _generalService.GenerateGuid();
                worker.Password = _cryptographyService.HashPassword(worker.Password);

                worker = await _unitOfWork.WorkerRepository.Add(worker);

                result.Success = true;
                result.Result = true;
                result.Message = WorkerRelatedMessages.WorkerRegistrationSuccess;
            }
            catch (Exception ex)
            {
                result.Message = WorkerRelatedMessages.WorkerExceptionError;
            }

            return result;
        }

        #endregion

        #region Login

        /// <summary>
        /// Validates login input, returns a worker instance
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<BaseResponse<WorkerDTO>> Login(LoginDTO loginDTO)
        {
            BaseResponse<WorkerDTO> response = new BaseResponse<WorkerDTO>();

            if(loginDTO != null)
            {
                // Check if email is empty
                if (string.IsNullOrEmpty(loginDTO.Email))
                {
                    response.Message = WorkerRelatedMessages.WorkerEmailEmptyError;
                    return response;
                }

                // Check if password is empty
                else if(string.IsNullOrEmpty(loginDTO.Password))
                {
                    response.Message = WorkerRelatedMessages.WorkerPasswordEmptyError;
                    return response;
                }

                // Get worker instance by email
                Worker worker = await _unitOfWork.WorkerRepository.GetByEmail(loginDTO.Email);

                // Worker found
                if(worker != null)
                {
                    // If password does not match
                    if(!_cryptographyService.VerifyPassword(loginDTO.Password, worker.Password))
                    {
                        response.Message = WorkerRelatedMessages.WorkerLoginEmailNotFoundError;
                        return response;
                    }

                    WorkerDTO workerInstance = _mapper.Map<WorkerDTO>(worker);
                    response.Success = true;
                    response.Result = workerInstance;
                }
                else
                {
                    response.Message = WorkerRelatedMessages.WorkerLoginEmailNotFoundError;
                }
            }

            return response;
        }

        #endregion

        #region Update Worker


        public async Task<BaseResponse<bool>> UpdateWorker(WorkerDTO workerDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            if(workerDTO != null)
            {
                Worker worker = await _unitOfWork.WorkerRepository.GetById(workerDTO.WorkerId);
                if(worker != null)
                {
                    worker.WorkerName = workerDTO.WorkerName;
                    worker.GenderId = workerDTO.GenderId;
                    worker.Email = workerDTO.Email;


                    await _unitOfWork.WorkerRepository.Update(worker);

                    response.Success = true;
                    response.Result = true;
                    response.Message = WorkerRelatedMessages.WorkerUpdateSuccess;
                }
                else
                {
                    response.Message = WorkerRelatedMessages.WorkerExceptionError;
                }
            }

            return response;
        }


        #endregion

        #endregion
    }
}
