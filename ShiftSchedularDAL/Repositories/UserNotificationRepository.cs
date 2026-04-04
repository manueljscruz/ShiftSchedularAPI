using Microsoft.EntityFrameworkCore;
using ShiftSchedularDAL.Data;
using ShiftSchedularDAL.IRepositories;
using ShiftSchedularDAL.UnitOfWork;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularDAL.Repositories
{
    public class UserNotificationRepository : GenericRepository<UserNotification>, IUserNotificationRepository
    {
        private readonly DataContext _context;
        private readonly DbSet<UserNotification> _dbSet;

        public UserNotificationRepository(DataContext context, IUnitOfWork unitOfWork) : base(context, unitOfWork)
        {
            _context = context;
            _dbSet = context.Set<UserNotification>();
        }

        #region Get Type By Code

        /// <summary>
        /// Fetches a NotificationType by its code string, including its Localizations.
        /// </summary>
        public async Task<NotificationType> GetTypeByCode(string code)
        {
            return await _context.NotificationTypes
                .Include(t => t.NotificationTypeLocalizations)
                .FirstOrDefaultAsync(t => t.NotificationTypeCode == code);
        }

        #endregion

        #region Get By User Id

        /// <summary>
        /// Returns a paginated list of notifications for a specific user, newest-first.
        /// </summary>
        public async Task<List<UserNotification>> GetByUserId(string userId, int page, int pageSize)
        {
            return await _dbSet
                .Include(n => n.NotificationType)
                    .ThenInclude(t => t.NotificationTypeLocalizations)
                .Where(n => n.UserId == userId)
                .OrderByDescending(n => n.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        #endregion

        #region Get Unread Count

        /// <summary>
        /// Returns the number of unread notifications for a user.
        /// </summary>
        public async Task<int> GetUnreadCount(string userId)
        {
            return await _dbSet
                .CountAsync(n => n.UserId == userId && !n.IsRead);
        }

        #endregion

        #region Get Total Count

        /// <summary>
        /// Returns the total number of notifications for a user (for pagination).
        /// </summary>
        public async Task<int> GetTotalCount(string userId)
        {
            return await _dbSet
                .CountAsync(n => n.UserId == userId);
        }

        #endregion

        #region Mark As Read

        /// <summary>
        /// Marks a single notification as read. Filters by both id AND userId for security.
        /// </summary>
        public async Task MarkAsRead(Guid id, string userId)
        {
            UserNotification notification = await _dbSet
                .FirstOrDefaultAsync(n => n.UserNotificationId == id && n.UserId == userId);

            if (notification != null && !notification.IsRead)
            {
                notification.IsRead = true;
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Mark All As Read

        /// <summary>
        /// Marks all notifications for a user as read.
        /// </summary>
        public async Task MarkAllAsRead(string userId)
        {
            List<UserNotification> unread = await _dbSet
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (unread.Count > 0)
            {
                foreach (UserNotification notification in unread)
                    notification.IsRead = true;

                await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
