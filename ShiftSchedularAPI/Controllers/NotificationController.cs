using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using System.Security.Claims;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("general")]
    public class NotificationController : ControllerBase
    {
        private readonly INotificationService _notificationService;

        #region Constructor

        public NotificationController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        #endregion

        #region Methods

        #region Get My Notifications

        /// <summary>
        /// Returns a paginated list of notifications for the authenticated user.
        /// </summary>
        [HttpGet("my")]
        [ProducesResponseType(200, Type = typeof(BaseResponse<List<UserNotificationDTO>>))]
        public async Task<IActionResult> GetMyNotifications([FromQuery] int page = 1, [FromQuery] int pageSize = 20)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            BaseResponse<List<UserNotificationDTO>> response = await _notificationService.GetMyNotifications(userId, page, pageSize);
            if (response.Success) return Ok(response);
            return BadRequest(response.Message);
        }

        #endregion

        #region Get Unread Count

        /// <summary>
        /// Returns the number of unread notifications for the authenticated user.
        /// Polled by the Angular client every 30 seconds.
        /// </summary>
        [HttpGet("unread-count")]
        [ProducesResponseType(200, Type = typeof(BaseResponse<int>))]
        public async Task<IActionResult> GetUnreadCount()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            BaseResponse<int> response = await _notificationService.GetUnreadCount(userId);
            if (response.Success) return Ok(response);
            return BadRequest(response.Message);
        }

        #endregion

        #region Mark As Read

        /// <summary>
        /// Marks a single notification as read. Scoped to the authenticated user.
        /// </summary>
        [HttpPut("mark-read/{id}")]
        [ProducesResponseType(200, Type = typeof(BaseResponse<bool>))]
        public async Task<IActionResult> MarkAsRead(Guid id)
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            BaseResponse<bool> response = await _notificationService.MarkAsRead(id, userId);
            if (response.Success) return Ok(response);
            return BadRequest(response.Message);
        }

        #endregion

        #region Mark All As Read

        /// <summary>
        /// Marks all notifications for the authenticated user as read.
        /// </summary>
        [HttpPut("mark-all-read")]
        [ProducesResponseType(200, Type = typeof(BaseResponse<bool>))]
        public async Task<IActionResult> MarkAllAsRead()
        {
            string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            BaseResponse<bool> response = await _notificationService.MarkAllAsRead(userId);
            if (response.Success) return Ok(response);
            return BadRequest(response.Message);
        }

        #endregion

        #endregion
    }
}
