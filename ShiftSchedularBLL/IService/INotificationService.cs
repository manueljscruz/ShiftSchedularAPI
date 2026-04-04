using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularBLL.IService
{
    public interface INotificationService
    {
        /// <summary>
        /// Creates a new notification for the specified user.
        /// Fire-and-forget: errors are logged and never propagate.
        /// </summary>
        Task CreateNotification(string userId, string typeCode, string? contextData = null, string? relatedEntityId = null);

        Task<BaseResponse<List<UserNotificationDTO>>> GetMyNotifications(string userId, int page, int pageSize);
        Task<BaseResponse<int>> GetUnreadCount(string userId);
        Task<BaseResponse<bool>> MarkAsRead(Guid id, string userId);
        Task<BaseResponse<bool>> MarkAllAsRead(string userId);
    }
}
