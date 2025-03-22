using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
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
        [ProducesResponseType(200, Type = typeof(ShiftDTO))]
        public async Task<IActionResult> GetShiftById(string id, string lcode)
        {
            var shifts = await _shiftService.GetShiftById(id, lcode);
            return Ok(shifts);
        }

        #endregion

        #region Get Entity Shifts View Model

        [HttpPost("get-entity-shift-view-model")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntityShiftsViewModel(BaseViewModelRequest viewModelRequestDTO)
        {
            if(viewModelRequestDTO == null)
            {
                return BadRequest();
            }

            var shifts = await _shiftService.GetEntityShiftsViewModel(viewModelRequestDTO);

            if(shifts == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "");
            }
            else
                return Ok(shifts);
        }

        #endregion

        #region Get Entity Shifts

        [HttpPost("get-entity-shifts")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetEntityShfits(SingleIdentifierDTO identifier)
        {
            var shifts = await _shiftService.GetEntityShifts(identifier.Identifier);
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
        public async Task<IActionResult> UpdateEntityShift([FromBody] ShiftDTO shift)
        {
            var updatedShift = await _shiftService.UpdateEntityShift(shift);
            return Ok(updatedShift);
        }

        #endregion

        #region Update Entity Shift Break

        [HttpPut("update-entity-shift-break")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateEntityShiftBreak([FromBody] ShiftBreakDTO shiftBreak)
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

        [HttpDelete("delete-entity-shift-break/{shiftBreakId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteShiftBreak(string shiftBreakId)
        {
            var response = await _shiftService.DeleteEntityShiftBreak(shiftBreakId);
            return Ok(response);
        }

        #endregion

        [HttpPost("get-shift-rotations")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntityShiftRotations(SingleIdentifierDTO identifier)
        {
            if (identifier.Identifier == Guid.Empty)
            {
                return BadRequest();
            }

            List<EntityShiftRotationDTO> rotations = await _shiftService.GetEntityShiftRotations(identifier.Identifier);

            return Ok(rotations);
        }

        [HttpPost("add-shift-rotation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddShiftRotation(AddShiftRotationDTO rotationDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BaseResponse<EntityShiftRotationDTO> response = await _shiftService.AddShiftRotation(rotationDTO);
            if (response.Success)
            {
                return Ok(response);
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            }
        }

        [HttpPut("update-shift-rotation")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateShiftRotation(UpdateShiftRotationDTO shiftRotationDTO)
        {
            if (shiftRotationDTO == null)
            {
                return BadRequest("");
            }

            BaseResponse<bool> response = await _shiftService.UpdateEntityShiftRotation(shiftRotationDTO);
            if (response.Success)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            }
        }

        [HttpDelete("delete-shift-rotation")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteShiftRotation(EntityShiftRotationDTO shiftRotationDTO)
        {
            if (shiftRotationDTO == null)
            {
                return BadRequest("");
            }

            BaseResponse<bool> response = await _shiftService.DeleteShiftRotation(shiftRotationDTO);
            if (response.Success)
            {
                return NoContent();
            }
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            }
        }
    }
}
