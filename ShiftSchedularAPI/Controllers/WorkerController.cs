using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WorkerController : ControllerBase
    {
        private readonly IWorkerService _workerService;

        #region Constructor

        public WorkerController(IWorkerService workerService)
        {
            _workerService = workerService;
        }

        #endregion

        #region Methods

        #region Add Worker

        /// <summary>
        /// Creates a new Worker entry in the database
        /// </summary>
        /// <param name="newWorkerDTO"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddWorker(NewWorkerDTO newWorkerDTO)
        {
            BaseResponse<bool> result = await _workerService.CreateWorker(newWorkerDTO);

            return Ok(result);
        }

        #endregion

        #region Login

        /// <summary>
        /// Login request, returns a worker instance
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            BaseResponse<WorkerDTO> result = await _workerService.Login(loginDTO);

            return Ok(result);
        }

        #endregion

        #endregion

    }
}
