using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Billing;
using System.Security.Claims;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("general")]
    public class EntityBillingController : ControllerBase
    {
        private readonly IEntityBillingService _entityBillingService;

        public EntityBillingController(IEntityBillingService entityBillingService)
        {
            _entityBillingService = entityBillingService;
        }

        #region Get Billing Summary

        [HttpGet("{entityId}/summary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetSummary(Guid entityId)
        {
            if (entityId == Guid.Empty)
                return BadRequest();

            string requesterId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            BaseResponse<BillingSummaryDTO> response = await _entityBillingService.GetBillingSummaryAsync(entityId, requesterId);

            if (response.Forbidden) return Forbid();
            if (response.NotFound) return NotFound(response.Message);
            if (!response.Success) return BadRequest(response.Message);
            return Ok(response.Result);
        }

        #endregion

        #region Get Subscription History

        [HttpGet("{entityId}/history")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetHistory(Guid entityId)
        {
            if (entityId == Guid.Empty)
                return BadRequest();

            string requesterId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            BaseResponse<List<SubscriptionHistoryItemDTO>> response = await _entityBillingService.GetSubscriptionHistoryAsync(entityId, requesterId);

            if (response.Forbidden) return Forbid();
            if (!response.Success) return BadRequest(response.Message);
            return Ok(response.Result);
        }

        #endregion

        #region Get Available Plans

        [HttpGet("available-plans")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailablePlans()
        {
            BaseResponse<List<AvailablePlanDTO>> response = await _entityBillingService.GetAvailablePlansAsync();

            if (!response.Success) return BadRequest(response.Message);
            return Ok(response.Result);
        }

        #endregion
    }
}
