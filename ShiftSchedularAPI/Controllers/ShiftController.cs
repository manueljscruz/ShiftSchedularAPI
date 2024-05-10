using Azure;
using Microsoft.AspNetCore.Http;
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

        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(200, Type = typeof(Shift))]
        public async Task<IActionResult> GetShiftById(string id)
        {
            var shifts = await _shiftService.GetShiftById(id);
            return Ok(shifts);
        }

        #endregion

        #region Get All Entity Shifts

        [HttpGet("get-all-entity-shifts/{entityId}/{lcode}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllEntityShifts(string entityId, string lcode)
        {
            var shifts = await _shiftService.GetAllEntityShifts(entityId, lcode);
            return Ok(shifts);
        }

        #endregion

        #region Add Entity Shift

        [HttpPost("add-entity-shift")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AddEntityShift(AddShiftDTO shiftDTO)
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

        [HttpDelete("delete-entity-shift/{entityId}/{shiftId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteShift(string entityId, string shiftId)
        {
            var response = await _shiftService.DeleteEntityShift(entityId, shiftId);
            return Ok(response);
        }

        [HttpDelete("delete-entity-shift-break")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteShiftBreak(DeleteEntityShiftBreakDTO deleteEntityShiftBreak)
        {
            var response = await _shiftService.DeleteEntityShiftBreak(deleteEntityShiftBreak);
            return Ok(response);
        }
    }
}
