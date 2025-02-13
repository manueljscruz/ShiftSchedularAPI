 using Microsoft.AspNetCore.Mvc;
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

        #region Constructor

        public UserController(IUserService userService)
        {
            _userService = userService;
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
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]    
        public async Task<IActionResult> RegisterUser(NewUserDTO newUserDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState); // Returns validation errors to the client
            }

            BaseResponse<bool> result = await _userService.CreateUser(newUserDTO);
            if(!result.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
            }


            return Ok(result);
        }

        #endregion

        #region Update User

        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateWorker(UserDTO userDTO)
        {
            if(userDTO == null)
            {
                return BadRequest();
            }

            BaseResponse<bool> result = await _userService.UpdateUser(userDTO);

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
