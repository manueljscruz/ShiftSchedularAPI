using AutoMapper;
using Microsoft.AspNetCore.Identity;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularRL.Resources.Home;

namespace ShiftSchedularBLL.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IMapper _mapper;

        public UserService(UserManager<ApplicationUser> userManager, IMapper mapper)
        {
            _userManager = userManager;
            _mapper = mapper;
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

        public async Task<BaseResponse<LoginResponseDTO>> Login(LoginDTO loginDTO)
        {
            BaseResponse<LoginResponseDTO> loginResponseDTO = new BaseResponse<LoginResponseDTO>();

            ApplicationUser user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if(user == null)
            {
                loginResponseDTO.Message = WorkerRelatedMessages.WorkerLoginEmailNotFoundError;
            }

            bool validLogin = await _userManager.CheckPasswordAsync(user, loginDTO.Password);
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
    }
}
