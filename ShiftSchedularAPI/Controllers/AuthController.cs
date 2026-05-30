using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.Extensions.Configuration;
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
        private readonly IAuthService _authService;
        private readonly IConfiguration _configuration;

        public AuthController(IAuthService authService, IConfiguration configuration)
        {
            _authService = authService;
            _configuration = configuration;
        }

        #region Login

        /// <summary>
        /// Logs in a user
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns></returns>
        [HttpPost("login")]
        [EnableRateLimiting("auth")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> Login([FromBody] LoginDTO loginDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BaseResponse<LoginResponseDTO> response = await _authService.Login(loginDTO);

            if (!response.Success)
            {
                return Unauthorized();
            }

            if(response.Success && response.Result.EmailConfirmed == false)
            {
                return Ok(new LoginResponseDTO
                {
                    User = response.Result.User,
                    EmailConfirmed = response.Result.EmailConfirmed,
                });
            }

            // Write cookies here
            Response.Cookies.Append("access_token", response.Result.TokenResponseDTO.Token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("refresh_token", response.Result.TokenResponseDTO.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(new LoginResponseDTO
            {
                User = response.Result.User,
                EmailConfirmed = response.Result.EmailConfirmed,
                IsAdmin = response.Result.IsAdmin
            });
        }

        #endregion

        #region Forgot Password

        [HttpPost("forgot-password")]
        [EnableRateLimiting("forgot-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequestDTO request)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            if (string.IsNullOrEmpty(request.Email))
                return BadRequest(WorkerRelatedMessages.WorkerEmailEmptyError);

            // Use configured frontend URL for reset link generation
            var frontendUrl = _configuration.GetValue<string>("FrontendUrl");
            if (string.IsNullOrEmpty(frontendUrl))
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Frontend URL is not configured." });
            }

            response = await _authService.ForgotPassword(request, frontendUrl);

            // Response will always be OK and message will also be the same
            // Never reveal if its an existing email or not
            return Ok(response);
        }

        #endregion

        #region Reset Password

        [HttpPost("reset-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequestDTO request)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            if (request == null)
                return BadRequest();

            response = await _authService.ResetPassword(request);

            if (response.Success)
                return Ok(response);

            return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        }

        #endregion

        #region Resend Confirmation Email

        [HttpPost("resend-confirmation-email")]
        [EnableRateLimiting("resend-confirmation")] // 3 attempts per 10 minutes
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
        public async Task<IActionResult> ResendConfirmationEmail([FromBody] ResendConfirmationEmailDTO request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            string frontendUrl = _configuration.GetValue<string>("FrontendUrl");
            if (string.IsNullOrEmpty(frontendUrl))
            {
                return StatusCode(StatusCodes.Status500InternalServerError,
                    new { message = "Frontend URL is not configured." });
            }

            BaseResponse<bool> response = await _authService.ResendConfirmationEmail(request, frontendUrl);

            if (response.Success)
                return Ok(response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        }

        #endregion

        #region Refresh Token

        /// <summary>
        /// Refresh a user's token
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [EnableRateLimiting("refresh")]
        [HttpPost("refresh-token")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> RefreshToken([FromBody] TokenModelDTO tokenModelDTO)
        {
            if(tokenModelDTO == null)
            {
                return BadRequest(WorkerRelatedMessages.TokensAreEmpty);
            }

            BaseResponse<TokenModelDTO> response = await _authService.RefreshToken(tokenModelDTO);
            if (!response.Success)
            {
                return BadRequest(response.Message);
            }

            Response.Cookies.Append("access_token", response.Result.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddMinutes(15)
            });

            Response.Cookies.Append("refresh_token", response.Result.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict,
                Expires = DateTimeOffset.UtcNow.AddDays(7)
            });

            return Ok(response);
        }

        #endregion

        #region Change Password

        [Authorize]
        [HttpPost("change-password")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequestDTO request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            string userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            BaseResponse<bool> response = await _authService.ChangePassword(userId, request);

            if (response.Success)
                return Ok(response);

            return BadRequest(response.Message);
        }

        #endregion

        #region Logout

        [Authorize]
        [EnableRateLimiting("logout")]
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = true;
            response.Message = "Logged out successfuly";

            return Ok(response);
        }

        #endregion
    }
}
