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

        #region Create Setup Intent

        [HttpPost("{entityId}/payment-methods/setup-intent")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> CreateSetupIntent(Guid entityId)
        {
            if (entityId == Guid.Empty)
                return BadRequest();

            string requesterId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            BaseResponse<SetupIntentDTO> response = await _entityBillingService.CreateSetupIntentAsync(entityId, requesterId);

            if (response.Forbidden) return Forbid();
            if (!response.Success) return BadRequest(response.Message);
            return Ok(response.Result);
        }

        #endregion

        #region Confirm Payment Method

        [HttpPost("{entityId}/payment-methods")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> ConfirmPaymentMethod(Guid entityId, [FromBody] ConfirmPaymentMethodDTO dto)
        {
            if (entityId == Guid.Empty || dto == null)
                return BadRequest();

            string requesterId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            BaseResponse<PaymentMethodSummaryDTO> response = await _entityBillingService.ConfirmPaymentMethodAsync(entityId, requesterId, dto);

            if (response.Forbidden) return Forbid();
            if (!response.Success) return BadRequest(response.Message);
            return Ok(response.Result);
        }

        #endregion

        #region Set Default Payment Method

        [HttpPost("{entityId}/payment-methods/{paymentMethodId}/default")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetDefaultPaymentMethod(Guid entityId, Guid paymentMethodId)
        {
            if (entityId == Guid.Empty || paymentMethodId == Guid.Empty)
                return BadRequest();

            string requesterId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            BaseResponse<bool> response = await _entityBillingService.SetDefaultPaymentMethodAsync(entityId, requesterId, paymentMethodId);

            if (response.Forbidden) return Forbid();
            if (response.NotFound) return NotFound(response.Message);
            if (!response.Success) return BadRequest(response.Message);
            return Ok();
        }

        #endregion

        #region Remove Payment Method

        [HttpDelete("{entityId}/payment-methods/{paymentMethodId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> RemovePaymentMethod(Guid entityId, Guid paymentMethodId)
        {
            if (entityId == Guid.Empty || paymentMethodId == Guid.Empty)
                return BadRequest();

            string requesterId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? string.Empty;
            BaseResponse<bool> response = await _entityBillingService.RemovePaymentMethodAsync(entityId, requesterId, paymentMethodId);

            if (response.Forbidden) return Forbid();
            if (response.NotFound) return NotFound(response.Message);
            if (!response.Success) return BadRequest(response.Message);
            return Ok();
        }

        #endregion
    }
}
