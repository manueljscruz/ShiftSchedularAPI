using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.APIManagement;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftTemplateController : ControllerBase
    {
        private readonly IShiftTemplateService _shiftTemplateService;

        #region Constructor

        public ShiftTemplateController(IShiftTemplateService shiftTemplateService)
        {
            _shiftTemplateService = shiftTemplateService;
        }

        #endregion

        #region Get Shift Template By Id

        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(200, Type = typeof(ShiftTemplate))]
        public async Task<IActionResult> GetShiftTemplateById(int id)
        {
            var shiftTemplate = await _shiftTemplateService.GetShiftTemplateById(id);
            return Ok(shiftTemplate);
        }

        #endregion

        #region Get Shift Templates

        [HttpGet("get-shift-templates/{lcode}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetEntityShiftTemplatesViewModel(string lcode)
        {
            var shifts = await _shiftTemplateService.GetShiftTemplates(lcode);
            return Ok(shifts);
        }

        #endregion

        #region Add Entity Shift Template

        [HttpPost("add-shift-template")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AddShiftTemplate(ShiftTemplateSubmissionModel submissionModel)
        {
            BaseResponse<int> response = await _shiftTemplateService.AddShiftTemplate(submissionModel);
            return Ok(response);
        }

        #endregion

        #region Add Entity Shift Template Break

        [HttpPost("add-shift-break-template")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddShiftBreakTemplate([FromBody] ShiftBreakTemplateSubmissionModel submissionModel)
        {
            BaseResponse<int> response = await _shiftTemplateService.AddShiftBreakTemplate(submissionModel);
            return Ok(response);
        }

        #endregion

        #region Update Shift Template

        [HttpPut("update-shift-template")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateEntityShiftTemplate(ShiftTemplateUpdateDTO shiftTemplateUpdateDTO)
        {
            BaseResponse<bool> response = await _shiftTemplateService.UpdateShiftTemplate(shiftTemplateUpdateDTO);
            return Ok(response);
        }

        #endregion

        #region Update Shift Break Template 

        [HttpPut("update-shift-break-template")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateShiftBreakTemplate(ShiftBreakTemplateDTO shiftBreakTemplateDTO)
        {
            BaseResponse<bool> response = await _shiftTemplateService.UpdateShiftBreakTemplate(shiftBreakTemplateDTO);
            return Ok(response);
        }

        #endregion

        #region Update Shift Template Pop Count

        [HttpPut("update-shift-template-pop-count/{shiftTemplateId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateShiftTemplatePopCount(int shiftTemplateId)
        {
            if (shiftTemplateId == 0)
                return BadRequest();


            BaseResponse<bool> response = await _shiftTemplateService.UpdateShiftTemplatePopCount(shiftTemplateId);

            if (response.Success)
                return NoContent();
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        }

        #endregion

        #region Update Shift Break Template Pop Count

        [HttpPut("update-shift-break-template-pop-count/{shiftBreakTemplateId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateShiftBreakTemplatePopCount(int shiftBreakTemplateId)
        {
            if (shiftBreakTemplateId == 0)
                return BadRequest();

            BaseResponse<bool> response = await _shiftTemplateService.UpdateShiftBreakTemplatePopCount(shiftBreakTemplateId);

            if (response.Success)
                return NoContent();
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        }

        #endregion

        #region Delete Shift Template

        [HttpDelete("delete-shift-template/{shiftTemplateId}")]
        public async Task<IActionResult> DeleteShiftTemplate(int shiftTemplateId)
        {
            BaseResponse<bool> response = await _shiftTemplateService.DeleteShiftTemplate(shiftTemplateId);
            return Ok(response);
        }

        #endregion

        #region Delete Shift Break Template

        [HttpDelete("delete-shift-break-template/{shiftBreakTemplateId}")]
        public async Task<IActionResult> DeleteShiftBreakTemplate(int shiftBreakTemplateId)
        {
            BaseResponse<bool> response = await _shiftTemplateService.DeleteShiftBreakTemplate(shiftBreakTemplateId);
            return Ok(response);
        }

        #endregion
    }
}
