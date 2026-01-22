using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;

        #region Constructor

        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }

        #endregion

        #region Methods

        #region Add User

        /// <summary>
        /// Creates a new user entry in the database
        /// </summary>
        /// <param name="newUserDTO"></param>
        /// <returns></returns>
        [HttpPost("register")]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]    
        public async Task<IActionResult> RegisterUser(NewUserDTO newUserDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Returns validation errors to the client
            }

            var frontendUrl = _configuration.GetValue<string>("FrontendUrl");
            if (string.IsNullOrEmpty(frontendUrl))
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Frontend URL is not configured." });
            }

            BaseResponse<bool> result = await _userService.CreateUser(newUserDTO, frontendUrl);
            if(!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
            }


            return Ok(result);
        }

        #endregion

        #region Confirm Email

        [HttpPost("confirm-email")]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> ConfirmEmail([FromBody]ConfirmEmailRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            BaseResponse<bool> result = await _userService.ConfirmEmail(request);

            if (result.Success)
                return Ok(result);
            else
                return BadRequest(result);

        }

        #endregion

        #region Update User

        [Authorize]
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateWorker(UserDTO userDTO)
        {
            if(userDTO == null)
            {
                return BadRequest();
            }

            var frontendUrl = _configuration.GetValue<string>("FrontendUrl");
            if (string.IsNullOrEmpty(frontendUrl))
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Frontend URL is not configured." });
            }

            BaseResponse<bool> result = await _userService.UpdateUser(userDTO, frontendUrl);

            if (!result.Success)
            {
                return BadRequest(result.Message);
            }

            return Ok(result);
        }

        #endregion

        #endregion

    }
}
