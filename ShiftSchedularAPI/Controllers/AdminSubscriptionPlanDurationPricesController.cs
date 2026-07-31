using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("general")]
    public class AdminSubscriptionPlanDurationPricesController : ControllerBase
    {
        private readonly ISubscriptionPlanDurationPriceService _service;

        public AdminSubscriptionPlanDurationPricesController(ISubscriptionPlanDurationPriceService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return Ok(await _service.GetAllAsync());
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var item = await _service.GetByIdAsync(id);
            if (item == null) return NotFound();
            return Ok(item);
        }

        [HttpPost]
        public async Task<IActionResult> Upsert([FromBody] AdminUpsertSubscriptionPlanDurationPriceDTO dto)
        {
            var response = await _service.UpsertAsync(dto);
            if (response.NotFound) return NotFound(response.Message);
            if (!response.Success) return BadRequest(response.Message);
            return Ok(new { id = response.Result });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var response = await _service.DeleteAsync(id);
            if (!response.Success) return BadRequest(response.Message);
            return NoContent();
        }
    }
}
