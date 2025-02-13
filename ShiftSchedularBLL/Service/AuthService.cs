using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularRL.Resources.Home;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace ShiftSchedularBLL.Service
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ITokenService _tokenService;

        public AuthService(UserManager<ApplicationUser> userManager, IMapper mapper, IConfiguration configuration, ITokenService tokenService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _configuration = configuration;
            _tokenService = tokenService;
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
            BaseResponse<LoginResponseDTO> loginResponseDTO = new BaseResponse<LoginResponseDTO>();

            ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if(user == null)
            {
                loginResponseDTO.Message = WorkerRelatedMessages.WorkerLoginEmailNotFoundError;
            }

            bool validLogin = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (validLogin)
            {
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

                user.RefreshTokenExpiryTime = DateTime.Now.AddMinutes(refreshTokenValidityInMinutes);

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
                loginResponseDTO.Message = WorkerRelatedMessages.WorkerLoginPasswordIncorrect;
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

        public Task<BaseResponse<bool>> UpdateUser()
        {
            throw new NotImplementedException();
        }

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

            if (user == null || user.RefreshToken != tokenModelDTO.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
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

    }
}
