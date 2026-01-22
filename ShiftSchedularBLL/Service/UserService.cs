using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularIL.IServices;
using ShiftSchedularRL.Resources.Home;
using ShiftSchedularRL.Resources.Shared;

namespace ShiftSchedularBLL.Service
{
    public class UserService : IUserService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IGeneralService _generalService;
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<UserService> _logger;

        #region Constructor

        public UserService(UserManager<ApplicationUser> userManager, IEmailService emailService, IGeneralService generalService, IMapper mapper, IUnitOfWork unitOfWork, ILogger<UserService> logger)
        {
            _userManager = userManager;
            _emailService = emailService;
            _generalService = generalService;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #endregion

        #region Methods

        #region Create User

        /// <summary>
        /// Creates a new user entry in the database
        /// </summary>
        /// <param name="newUserDTO"></param>
        /// <returns></returns>
        public async Task<BaseResponse<bool>> CreateUser(NewUserDTO newUserDTO, string originLink)
        {
            _logger.LogInformation("User registration attempt for email: {Email}", newUserDTO.Email);
            BaseResponse<bool> response = new BaseResponse<bool>();

            var existingUser = await _userManager.FindByEmailAsync(newUserDTO.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration failed: Email already in use: {Email}", newUserDTO.Email);
                response.Message = WorkerRelatedMessages.WorkerEmailInUseError;
                return response;
            }

            ApplicationUser newUser = _mapper.Map<ApplicationUser>(newUserDTO);
            newUser.UserName = newUserDTO.Email.Split("@")[0];

            IdentityResult result = await _userManager.CreateAsync(newUser, newUserDTO.Password);

            if (result.Succeeded)
            {
                _logger.LogInformation("User created successfully: {UserId} ({Email})", newUser.Id, newUser.Email);
                await _userManager.AddToRoleAsync(newUser, "User");
                var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(newUser);

                string resetLink = $"{originLink}/confirm-email?token={Uri.EscapeDataString(confirmEmailToken)}&email={Uri.EscapeDataString(newUser.Email)}";
                await _emailService.SendConfirmEmail(WorkerRelatedMessages.ConfirmEmailEmailSubject, newUser, resetLink);

                _logger.LogInformation("Confirmation email sent to: {Email}", newUser.Email);

                response.Result = true;
                response.Message = WorkerRelatedMessages.WorkerRegistrationSuccess;
                response.Success = true;
            }
            else
            {
                _logger.LogError("User creation failed for email: {Email}. Errors: {Errors}",
                    newUserDTO.Email, string.Join(", ", result.Errors.Select(e => e.Description)));
                response.Result = false;
                response.Message = WorkerRelatedMessages.WorkerExceptionError;
            }

            return response;
        }

        #endregion

        #region Confirm Email

        public async Task<BaseResponse<bool>> ConfirmEmail(ConfirmEmailRequestDTO request)
        {
            _logger.LogInformation("Email confirmation attempt for: {Email}", request.Email);
            BaseResponse<bool> response = new BaseResponse<bool>();

            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);

            // Prevent email enumeration - return success even if user not found
            if(user == null)
            {
                _logger.LogWarning("Email confirmation failed: User not found for email: {Email}", request.Email);
                response.Success = true;
                response.Message = WorkerRelatedMessages.EmailConfirmationProcessed;
                return response;
            }

            // If email is already confirmed, return success
            if (user.EmailConfirmed)
            {
                _logger.LogInformation("Email confirmation attempt for already confirmed email: {Email} (UserId: {UserId})", request.Email, user.Id);
                response.Success = true;
                response.Message = WorkerRelatedMessages.EmailAlreadyConfirmed;
                return response;
            }

            IdentityResult result = await _userManager.ConfirmEmailAsync(user, request.Token);
            if (result.Succeeded)
            {
                _logger.LogInformation("Email confirmed successfully for: {Email} (UserId: {UserId})", request.Email, user.Id);
                response.Success = true;
                response.Message = WorkerRelatedMessages.EmailConfirmed;
            }
            else
            {
                _logger.LogWarning("Email confirmation failed for: {Email} (UserId: {UserId}). Errors: {Errors}",
                    request.Email, user.Id, string.Join(", ", result.Errors.Select(e => e.Description)));
                response.Message = string.Join(", ", result.Errors.Select(e => e.Description));
                return response;
            }

            return response;
        }

        #endregion

        #region Update Worker

        public async Task<BaseResponse<bool>> UpdateUser(UserDTO userDTO, string frontendUrl)
        {
            _logger.LogInformation("User update attempt for UserId: {UserId}", userDTO.UserId);
            BaseResponse<bool> response = new BaseResponse<bool>();

            ApplicationUser applicationUser = await _userManager.FindByIdAsync(userDTO.UserId);
            if (applicationUser == null)
            {
                _logger.LogWarning("User update failed: User not found with UserId: {UserId}", userDTO.UserId);
                response.Message = WorkerRelatedMessages.WorkerExceptionError;
                return response;
            }

            // Set display name and gender values
            applicationUser.DisplayName = userDTO.UserDisplayName;
            applicationUser.GenderId = userDTO.GenderId;

            // Track if email changed to send confirmation email
            bool emailChanged = false;
            string oldEmail = applicationUser.Email;
            if (applicationUser.Email != userDTO.Email)
            {
                emailChanged = true;
                applicationUser.Email = userDTO.Email;
                applicationUser.UserName = userDTO.Email.Split("@")[0];
                applicationUser.EmailConfirmed = false;
                _logger.LogInformation("Email change detected for UserId: {UserId}. Old: {OldEmail}, New: {NewEmail}",
                    userDTO.UserId, oldEmail, userDTO.Email);
            }

            // Update user
            IdentityResult result = await _userManager.UpdateAsync(applicationUser);
            if(result.Succeeded)
            {
                _logger.LogInformation("User updated successfully: {UserId} ({Email})", applicationUser.Id, applicationUser.Email);

                // Send confirmation email if email was changed
                if (emailChanged)
                {
                    var confirmEmailToken = await _userManager.GenerateEmailConfirmationTokenAsync(applicationUser);
                    string confirmLink = $"{frontendUrl}/confirm-email?token={Uri.EscapeDataString(confirmEmailToken)}&email={Uri.EscapeDataString(applicationUser.Email)}";
                    await _emailService.SendConfirmEmail(WorkerRelatedMessages.ConfirmEmailEmailSubject, applicationUser, confirmLink);

                    _logger.LogInformation("Confirmation email sent to new email address: {Email} (UserId: {UserId})",
                        applicationUser.Email, applicationUser.Id);

                    response.Message = WorkerRelatedMessages.WorkerUpdateSuccessEmailSent;
                }
                else
                {
                    response.Message = WorkerRelatedMessages.WorkerUpdateSuccess;
                }

                response.Success = true;
                response.Result = true;
            }
            else
            {
                _logger.LogError("User update failed for UserId: {UserId}. Errors: {Errors}",
                    userDTO.UserId, string.Join(", ", result.Errors.Select(e => e.Description)));
                response.Message = WorkerRelatedMessages.WorkerExceptionError;
            }

            return response;
        }

        #endregion

        #endregion
    }
}
