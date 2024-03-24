using AutoMapper;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.Home;

namespace ShiftSchedularBLL.Service
{
    public class WorkerService : IWorkerService
    {
        private readonly ICryptographyService _cryptographyService;
        private readonly IGeneralService _generalService;
        private readonly IWorkerRepository _workerRepository;
        private readonly IMapper _mapper;


        #region Constructor

        public WorkerService(ICryptographyService cryptographyService, IGeneralService generalService, IWorkerRepository workerRepository, IMapper mapper)
        {
            _cryptographyService = cryptographyService;
            _generalService = generalService;
            _workerRepository = workerRepository;
            _mapper = mapper;
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
                if(string.IsNullOrEmpty(newWorker.WorkerName))
                {
                    result.Message = Home.WorkerRegistrationNameEmptyError;
                    return result;
                }

                if(string.IsNullOrEmpty(newWorker.Password))
                {
                    result.Message = Home.WorkerRegistrationPasswordEmptyError;
                    return result;
                }

                if(string.IsNullOrEmpty(newWorker.Email))
                {
                    result.Message = Home.WorkerEmailEmptyError;
                    return result;
                }

                if (!_generalService.ValidateRegexEmail(newWorker.Email))
                {
                    result.Message = Home.WorkerRegistrationEmailInvalidError;
                    return result;
                }

                Worker worker = _mapper.Map<Worker>(newWorker);
                worker.IsActive = true;
                worker.WorkerId = _generalService.GenerateGuid();
                worker.Password = _cryptographyService.HashPassword(worker.Password);

                worker = await _workerRepository.Add(worker);

                result.Success = true;
                result.Result = true;
                result.Message = Home.WorkerRegistrationSuccess;
            }
            catch (Exception ex)
            {
                result.Message = Home.WorkerRegistrationExceptionError;
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
                    response.Message = Home.WorkerEmailEmptyError;
                    return response;
                }

                // Check if password is empty
                else if(string.IsNullOrEmpty(loginDTO.Password))
                {
                    response.Message = Home.WorkerPasswordEmptyError;
                    return response;
                }

                // Get worker instance by email
                Worker worker = await _workerRepository.GetByEmail(loginDTO.Email);

                // Worker found
                if(worker != null)
                {
                    // If password does not match
                    if(!_cryptographyService.VerifyPassword(loginDTO.Password, worker.Password))
                    {
                        response.Message = Home.WorkerLoginEmailNotFoundError;
                        return response;
                    }

                    WorkerDTO workerInstance = _mapper.Map<WorkerDTO>(worker);
                    response.Success = true;
                    response.Result = workerInstance;
                }
                else
                {
                    response.Message = Home.WorkerLoginEmailNotFoundError;
                }
            }

            return response;
        }


        #endregion

        #endregion
    }
}
