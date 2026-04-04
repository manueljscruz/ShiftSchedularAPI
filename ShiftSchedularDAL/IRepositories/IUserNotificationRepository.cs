using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.IRepositories
{
    public interface IUserNotificationRepository : IGenericRepository<UserNotification>
    {
        Task<NotificationType> GetTypeByCode(string code);
        Task<List<UserNotification>> GetByUserId(string userId, int page, int pageSize);
        Task<int> GetUnreadCount(string userId);
        Task<int> GetTotalCount(string userId);
        Task MarkAsRead(Guid id, string userId);
        Task MarkAllAsRead(string userId);
    }
}
