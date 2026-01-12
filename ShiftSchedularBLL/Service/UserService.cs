using AutoMapper;
using Microsoft.AspNetCore.Identity;
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
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ICryptographyService _cryptographyService;
        private readonly IGeneralService _generalService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;

        #region Constructor

        public UserService(UserManager<ApplicationUser> userManager, ICryptographyService cryptographyService, IGeneralService generalService, IMapper mapper, IUnitOfWork unitOfWork)
        {
            _userManager = userManager;
            _cryptographyService = cryptographyService;
            _generalService = generalService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }

        #endregion

        #region Methods

        #region Create User

        /// <summary>
        /// Creates a new user entry in the database
        /// </summary>
        /// <param name="newUserDTO"></param>
        /// <returns></returns>
        public async Task<BaseResponse<bool>> CreateUser(NewUserDTO newUserDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            var existingUser = await _userManager.FindByEmailAsync(newUserDTO.Email);
            if (existingUser != null)
            {
                response.Message = WorkerRelatedMessages.WorkerEmailInUseError;
                return response;
            }

            ApplicationUser newUser = _mapper.Map<ApplicationUser>(newUserDTO);
            newUser.UserName = newUserDTO.Email.Split("@")[0];



            IdentityResult result = await _userManager.CreateAsync(newUser, newUserDTO.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, "User");

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

        #region Confirm Email

        public Task<BaseResponse<bool>> ConfirmEmail(string email, string token)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Update Worker

        public async Task<BaseResponse<bool>> UpdateUser(UserDTO userDTO)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();

            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userDTO.UserId);
            if (applicationUser == null)
            {
                response.Message = WorkerRelatedMessages.WorkerExceptionError;
                return response;
            }

            // Set display name and gender values
            applicationUser.DisplayName = userDTO.UserDisplayName;
            applicationUser.GenderId = userDTO.GenderId;

            // if email value is different, update email and username
            if (applicationUser.Email != userDTO.Email)
            {
                applicationUser.Email = userDTO.Email;
                applicationUser.UserName = userDTO.Email.Split("@")[0];
                applicationUser.EmailConfirmed = false;
            }           

            // Update user
            IdentityResult result = await _userManager.UpdateAsync(applicationUser);
            if(result.Succeeded)
            {
                response.Success = true;
                response.Result = true;
                response.Message = WorkerRelatedMessages.WorkerUpdateSuccess;
            }
            else
            {
                response.Message = WorkerRelatedMessages.WorkerExceptionError;
            }

            return response;
        }

        #endregion

        #endregion
    }
}
