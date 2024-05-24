using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftController : ControllerBase
    {
        private readonly IShiftService _shiftService;

        public ShiftController(IShiftService shiftService)
        {
            _shiftService = shiftService;
        }

        #region Get Shift By Id

        [HttpGet("get-by-id/{id}/{lcode}")]
        [ProducesResponseType(200, Type = typeof(Shift))]
        public async Task<IActionResult> GetShiftById(string id, string lcode)
        {
            var shifts = await _shiftService.GetShiftById(id, lcode);
            return Ok(shifts);
        }

        #endregion

        #region Get Entity Shifts View Model

        [HttpPost("get-entity-shift-view-model")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetEntityShiftsViewModel(EntityShiftViewModelRequestDTO viewModelRequestDTO)
        {
            var shifts = await _shiftService.GetEntityShiftsViewModel(viewModelRequestDTO);
            return Ok(shifts);
        }

        #endregion

        #region Add Entity Shift

        [HttpPost("add-entity-shift")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AddEntityShift([FromBody] AddShiftDTO shiftDTO)
        {
            var newShift = await _shiftService.AddEntityShift(shiftDTO);
            return Ok(newShift);
        }

        #endregion

        #region Add Entity Shift Break

        [HttpPost("add-entity-shift-break")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddEntityShiftBreak(AddShiftBreakDTO shiftBreakDTO)
        {
            var newShiftBreak = await _shiftService.AddEntityShiftBreak(shiftBreakDTO);
            return Ok(newShiftBreak);
        }

        #endregion

        #region Update Entity Shift

        [HttpPut("update-entity-shift")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateEntityShift(ShiftDTO shift)
        {
            var updatedShift = await _shiftService.UpdateEntityShift(shift);
            return Ok(updatedShift);
        }

        #endregion

        #region Update Entity Shift Break

        [HttpPut("update-entity-shift-break")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateEntityShiftBreak(ShiftBreakDTO shiftBreak)
        {
            var updatedShiftBreak = await _shiftService.UpdateEntityShiftBreak(shiftBreak);
            return Ok(updatedShiftBreak);
        }

        #endregion

        #region Delete Shift

        [HttpDelete("delete-entity-shift/{entityId}/{shiftId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteShift(string entityId, string shiftId)
        {
            var response = await _shiftService.DeleteEntityShift(entityId, shiftId);
            return Ok(response);
        }

        #endregion

        #region Delete Shift Break

        [HttpDelete("delete-entity-shift-break")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteShiftBreak(DeleteEntityShiftBreakDTO deleteEntityShiftBreak)
        {
            var response = await _shiftService.DeleteEntityShiftBreak(deleteEntityShiftBreak);
            return Ok(response);
        }

        #endregion
    }
}
