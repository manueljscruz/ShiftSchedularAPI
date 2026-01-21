using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.Home;
using ShiftSchedularRL.Resources.LogMessages;
using ShiftSchedularRL.Resources.Shared;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShiftSchedularBLL.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(UserManager<ApplicationUser> userManager, IEmailService emailService, IMapper mapper, IConfiguration configuration, ITokenService tokenService, ILogger<AuthService> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _mapper = mapper;
            _configuration = configuration;
            _tokenService = tokenService;
            _logger = logger;
        }

        #region Register

        /// <summary>
        /// Registration of a new user
        /// </summary>
        /// <param name="newUserDTO"></param>
        /// <returns></returns>
        public async Task<BaseResponse<bool>> Register(NewUserDTO newUserDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            var existingUser = await _userManager.FindByEmailAsync(newUserDTO.Email);
            if(existingUser != null)
            {
                response.Message = WorkerRelatedMessages.WorkerEmailInUseError;
                return response;
            }

            ApplicationUser newUser = _mapper.Map<ApplicationUser>(newUserDTO);
            newUser.UserName = newUserDTO.Email.Split("@")[0];

            IdentityResult result = await _userManager.CreateAsync(newUser, newUserDTO.Password);
            if(result.Succeeded)
            {
                response.Result = true;
                response.Message = WorkerRelatedMessages.WorkerRegistrationSuccess;
                response.Success = true;
            }
            else
            {
                response.Result = false;
                response.Message = WorkerRelatedMessages.WorkerExceptionError;
            }

            return response;
        }

        #endregion

        #region Login

        /// <summary>
        /// Login of a user
        /// </summary>
        /// <param name="loginDTO"></param>
        /// <returns></returns>
        public async Task<BaseResponse<LoginResponseDTO>> Login(LoginDTO loginDTO)
        {
            _logger.LogInformation("Login attempt for email: {Email}", loginDTO.Email);
            BaseResponse<LoginResponseDTO> loginResponseDTO = new BaseResponse<LoginResponseDTO>();
            try
            {
                ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.Email);
                if (user == null)
                {
                    _logger.LogWarning("Login failed: User not found for email: {Email}", loginDTO.Email);
                    loginResponseDTO.Message = WorkerRelatedMessages.WorkerLoginEmailNotFoundError;
                    return loginResponseDTO;
                }

                bool validLogin = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
                if (validLogin)
                {
                    _logger.LogInformation("Successful login for user: {UserId} ({Email})", user.Id, loginDTO.Email);
                    var userRoles = await _userManager.GetRolesAsync(user);

                    var authClaims = new List<Claim>
                    {
                        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                    };

                    foreach (var userRole in userRoles)
                    {
                        authClaims.Add(new Claim(ClaimTypes.Role, userRole));
                    }

                    var token = _tokenService.GenerateAccessToken(authClaims, _configuration);

                    var refreshToken = _tokenService.GenerateRefreshToken();

                    _ = int.TryParse(_configuration["Jwt:RefreshTokenValidityInMinutes"], out int refreshTokenValidityInMinutes);

                    user.RefreshToken = refreshToken;

                    user.RefreshTokenExpiryTime = DateTime.UtcNow.AddMinutes(refreshTokenValidityInMinutes);

                    await _userManager.UpdateAsync(user);

                    loginResponseDTO.Result = new LoginResponseDTO
                    {
                        User = _mapper.Map<UserDTO>(user),
                        TokenResponseDTO = new TokenResponseDTO(new JwtSecurityTokenHandler().WriteToken(token), refreshToken, token.ValidTo)
                    };
                    loginResponseDTO.Success = true;
                }
                else
                {
                    _logger.LogWarning("Login failed: Invalid password for email: {Email}", loginDTO.Email);
                    loginResponseDTO.Message = WorkerRelatedMessages.WorkerLoginPasswordIncorrect;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login error for email: {Email}", loginDTO.Email);
                loginResponseDTO.Message = ex.Message;
            }

            return loginResponseDTO;
        }

        #endregion

        #region Confirm Email

        public Task<BaseResponse<bool>> ConfirmEmail(string email, string token)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Refresh Token

        public async Task<BaseResponse<TokenModelDTO>> RefreshToken(TokenModelDTO tokenModelDTO)
        {
            BaseResponse<TokenModelDTO> response = new BaseResponse<TokenModelDTO>();

            if (string.IsNullOrEmpty(tokenModelDTO.AccessToken) || string.IsNullOrEmpty(tokenModelDTO.RefreshToken))
            {
                response.Message = WorkerRelatedMessages.TokensAreEmpty;
                return response;
            }

            var principal = _tokenService.GetPrincipalFromExpiredToken(tokenModelDTO.AccessToken, _configuration);

            if(principal == null)
            {
                response.Message = WorkerRelatedMessages.InvalidTokens;
                return response;
            }

            string userName = principal.Identity.Name;

            var user = await _userManager.FindByNameAsync(userName!);

            if (user == null || user.RefreshToken != tokenModelDTO.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            {
                response.Message = WorkerRelatedMessages.InvalidTokens;
                return response;
            }

            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims.ToList(), _configuration);

            var newRefreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);

            response.Result = new TokenModelDTO
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(newAccessToken),
                RefreshToken = newRefreshToken
            };

            response.Success = true;
            return response;
        }

        #endregion

        #region Forgot Password

        public async Task<BaseResponse<bool>> ForgotPassword(ForgotPasswordRequestDTO request, string originRequest)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Message = WorkerRelatedMessages.ForgotPasswordMessage;
            response.Success = true;

            var user = await _userManager.FindByEmailAsync(request.Email);

            // No email was found in the DB
            if(user == null)
            {
                _logger.LogInformation(string.Format(LogMessages.BadEmailForgotPassword, request.Email));
                return response;
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            string resetLink = $"{originRequest}/reset-password?token={Uri.EscapeDataString(token)}&email={Uri.EscapeDataString(user.Email)}";

            await _emailService.SendForgotPasswordEmail(WorkerRelatedMessages.ForgotPasswordEmailSubject, user, resetLink);

            return response;
        }

        #endregion

        #region Reset Password

        public async Task<BaseResponse<bool>> ResetPassword(ResetPasswordRequestDTO request)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Message = SharedMessages.UnexpectedError;

            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);

            if(user == null)
            {
                _logger.LogInformation(string.Format(LogMessages.BadEmailForgotPassword, request.Email));
                response.Message = WorkerRelatedMessages.WorkerLoginEmailNotFoundError;
                return response;
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);

            if (result.Succeeded)
            {
                response.Success = true;
                response.Message = WorkerRelatedMessages.PasswordResetSuccess;
            }
            else
            {
                response.Message = string.Join("; ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Password reset failed for email: {Email}. Errors: {Errors}", request.Email, response.Message);
            }
            

            return response;
        }

        #endregion

    }
}
