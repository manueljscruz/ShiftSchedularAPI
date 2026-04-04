using Microsoft.Extensions.Logging;
using ShiftSchedularBLL.IService;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using System.Threading.Tasks;

namespace ShiftSchedularBLL.Service
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<NotificationService> _logger;
        private readonly ILanguageAccessor _languageAccessor;

        // Always resolve message templates in English for now.
        // Locale-per-user can be added later when user language preferences are persisted.
        private const string DefaultLocale = "en";

        #region Constructor

        public NotificationService(IUnitOfWork unitOfWork, ILogger<NotificationService> logger, ILanguageAccessor languageAccessor)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _languageAccessor = languageAccessor;
        }

        #endregion

        #region Create Notification

        /// <summary>
        /// Resolves the localized message template, substitutes context data if needed,
        /// and persists a new UserNotification. Designed to be fire-and-forget.
        /// </summary>
        public async Task CreateNotification(string userId, string typeCode, string? contextData = null, string? relatedEntityId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(typeCode))
                    return;

                NotificationType notifType = await _unitOfWork.UserNotificationRepository.GetTypeByCode(typeCode);
                if (notifType == null)
                {
                    _logger.LogWarning("NotificationService: unknown typeCode '{TypeCode}'", typeCode);
                    return;
                }

                Localization localization = await _unitOfWork.LocalizationRepository.GetLocalizationByLanguageCode(_languageAccessor.GetLanguageCode());

                // Prefer user locale; fall back to first available
                NotificationTypeLocalization notificationTypeLocalized = localization != null
                    ? notifType.NotificationTypeLocalizations.FirstOrDefault(l => l.LocalizationId == localization.LocalizationId)
                        ?? notifType.NotificationTypeLocalizations.FirstOrDefault()
                    : notifType.NotificationTypeLocalizations.FirstOrDefault();

                if (notificationTypeLocalized == null)
                {
                    _logger.LogWarning("NotificationService: no localization found for typeCode '{TypeCode}'", typeCode);
                    return;
                }

                string message = notificationTypeLocalized.MessageTemplate.Contains("{0}")
                    ? string.Format(notificationTypeLocalized.MessageTemplate, contextData ?? string.Empty)
                    : notificationTypeLocalized.MessageTemplate;

                UserNotification notification = new UserNotification
                {
                    UserId = userId,
                    NotificationTypeId = notifType.NotificationTypeId,
                    ContextData = contextData,
                    RelatedEntityId = relatedEntityId,
                    IsRead = false,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.UserNotificationRepository.Add(notification);
                await _unitOfWork.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NotificationService: failed to create notification for user '{UserId}', type '{TypeCode}'", userId, typeCode);
            }
        }

        #endregion

        #region Get My Notifications

        /// <summary>
        /// Returns a paginated list of notifications for the requesting user, with pre-formatted messages.
        /// </summary>
        public async Task<BaseResponse<List<UserNotificationDTO>>> GetMyNotifications(string userId, int page, int pageSize)
        {
            BaseResponse<List<UserNotificationDTO>> response = new BaseResponse<List<UserNotificationDTO>>();
            response.Success = false;

            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    response.Message = "User identifier is required.";
                    return response;
                }

                List<UserNotification> notifications = await _unitOfWork.UserNotificationRepository.GetByUserId(userId, page, pageSize);

                Localization localization = await _unitOfWork.LocalizationRepository.GetLocalizationByLanguageCode(_languageAccessor.GetLanguageCode());
                response.Result = notifications.Select(n => MapToDTO(n, localization)).ToList();
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NotificationService: failed to get notifications for user '{UserId}'", userId);
                response.Message = ex.Message;
            }

            return response;
        }

        #endregion

        #region Get Unread Count

        public async Task<BaseResponse<int>> GetUnreadCount(string userId)
        {
            BaseResponse<int> response = new BaseResponse<int>();
            response.Success = false;

            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    response.Message = "User identifier is required.";
                    return response;
                }

                response.Result = await _unitOfWork.UserNotificationRepository.GetUnreadCount(userId);
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NotificationService: failed to get unread count for user '{UserId}'", userId);
                response.Message = ex.Message;
            }

            return response;
        }

        #endregion

        #region Mark As Read

        public async Task<BaseResponse<bool>> MarkAsRead(Guid id, string userId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;

            try
            {
                await _unitOfWork.UserNotificationRepository.MarkAsRead(id, userId);
                response.Result = true;
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NotificationService: failed to mark notification {Id} as read for user '{UserId}'", id, userId);
                response.Message = ex.Message;
            }

            return response;
        }

        #endregion

        #region Mark All As Read

        public async Task<BaseResponse<bool>> MarkAllAsRead(string userId)
        {
            BaseResponse<bool> response = new BaseResponse<bool>();
            response.Success = false;

            try
            {
                await _unitOfWork.UserNotificationRepository.MarkAllAsRead(userId);
                response.Result = true;
                response.Success = true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "NotificationService: failed to mark all notifications as read for user '{UserId}'", userId);
                response.Message = ex.Message;
            }

            return response;
        }

        #endregion

        #region Private Helpers

        private UserNotificationDTO MapToDTO(UserNotification n, Localization localization)
        {
            string message = string.Empty;

            NotificationTypeLocalization notificationTypeLocalization = localization != null
                ? n.NotificationType?.NotificationTypeLocalizations?.FirstOrDefault(l => l.LocalizationId == localization.LocalizationId)
                    ?? n.NotificationType?.NotificationTypeLocalizations?.FirstOrDefault()
                : n.NotificationType?.NotificationTypeLocalizations?.FirstOrDefault();

            if (notificationTypeLocalization != null)
            {
                message = notificationTypeLocalization.MessageTemplate.Contains("{0}")
                    ? string.Format(notificationTypeLocalization.MessageTemplate, n.ContextData ?? string.Empty)
                    : notificationTypeLocalization.MessageTemplate;
            }

            return new UserNotificationDTO
            {
                Id = n.UserNotificationId,
                NotificationTypeCode = n.NotificationType?.NotificationTypeCode ?? string.Empty,
                Message = message,
                RelatedEntityId = n.RelatedEntityId,
                IsRead = n.IsRead,
                CreatedAt = n.CreatedAt
            };
        }

        #endregion
    }
}
