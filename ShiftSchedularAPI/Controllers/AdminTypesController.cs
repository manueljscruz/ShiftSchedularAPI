using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShiftSchedularBLL.Admin;
using ShiftSchedularEntity.Models.DataTransferObjects.Admin;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("general")]
    public class AdminTypesController : ControllerBase
    {
        private readonly IServiceProvider _serviceProvider;

        public AdminTypesController(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        private IAdminTypeHandler? GetHandler(string typeKey)
        {
            return _serviceProvider.GetKeyedService<IAdminTypeHandler>(typeKey);
        }

        [HttpGet("{typeKey}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAll(string typeKey)
        {
            var handler = GetHandler(typeKey);
            if (handler == null) return NotFound($"Unknown type key: {typeKey}");

            var items = await handler.GetAllAsync();
            return Ok(new { hasColors = handler.HasColors, items });
        }

        [HttpGet("{typeKey}/{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetById(string typeKey, int id)
        {
            var handler = GetHandler(typeKey);
            if (handler == null) return NotFound($"Unknown type key: {typeKey}");

            var item = await handler.GetByIdAsync(id);
            if (item == null) return NotFound();

            return Ok(item);
        }

        [HttpPost("{typeKey}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Upsert(string typeKey, [FromBody] AdminUpsertTypeDTO dto)
        {
            var handler = GetHandler(typeKey);
            if (handler == null) return NotFound($"Unknown type key: {typeKey}");

            if (string.IsNullOrWhiteSpace(dto.InternalName))
                return BadRequest("InternalName is required.");

            var id = await handler.UpsertAsync(dto);
            if (id == 0) return BadRequest("Upsert failed.");

            return Ok(new { id });
        }

        [HttpDelete("{typeKey}/{id:int}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Delete(string typeKey, int id)
        {
            var handler = GetHandler(typeKey);
            if (handler == null) return NotFound($"Unknown type key: {typeKey}");

            await handler.DeleteAsync(id);
            return NoContent();
        }
    }
}
