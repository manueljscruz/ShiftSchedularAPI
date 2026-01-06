using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularRL.Resources.Home;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _userService;

        public AuthController(IAuthService userService)
        {
            _userService = userService;
        }

        #region Login

        /// <summary>
        /// Logs in a user
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BaseResponse<LoginResponseDTO> response = await _userService.Login(loginDTO);

            if (!response.Success)
            {
                return Unauthorized();
            }

            // Write cookies here
            Response.Cookies.Append("access_token", response.Result.TokenResponseDTO.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("refresh_token", response.Result.TokenResponseDTO.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(new LoginResponseDTO
            {
                User = response.Result.User
            });
        }

        #endregion

        #region Refresh Token

        /// <summary>
        /// Refresh a user's token
        /// </summary>
        /// <returns></returns>
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModelDTO tokenModelDTO)
        {
            if(tokenModelDTO == null)
            {
                return BadRequest(WorkerRelatedMessages.TokensAreEmpty);
            }

            BaseResponse<TokenModelDTO> response = await _userService.RefreshToken(tokenModelDTO);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            return Ok(response);
        }

        #endregion
    }
}
