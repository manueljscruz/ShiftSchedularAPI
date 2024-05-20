using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
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

        #region Get Entity Shift Templates View Model

        //[HttpPost("get-entity-shift-templates-view-model")]
        //[ProducesResponseType(200)]
        //public async Task<IActionResult> GetEntityShiftTemplatesViewModel(EntityShiftTemplateViewModelRequestDTO viewModelRequestDTO)
        //{
        //    var shifts = await _shiftTemplateService.GetEntityShiftTemplatesViewModel(viewModelRequestDTO);
        //    return Ok(shifts);
        //}

        #endregion

        #region Add Entity Shift Template

        [HttpPost("add-shift-template")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AddShiftTemplate(ShiftTemplateSubmissionModel submissionModel)
        {
            var newShiftTemplate = await _shiftTemplateService.AddShiftTemplate(submissionModel);
            return Ok(newShiftTemplate);
        }

        #endregion

        #region Add Entity Shift Template Break

        [HttpPost("add-shift-break-template")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddShiftBreakTemplate(ShiftBreakTemplateSubmissionModel submissionModel)
        {
            var newShiftBreakTemplate = await _shiftTemplateService.AddShiftBreakTemplate(submissionModel);
            return Ok(newShiftBreakTemplate);
        }

        #endregion

        #region Update Shift Template

        [HttpPut("update-shift-template")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateEntityShiftTemplate(ShiftTemplateUpdateDTO shiftTemplateUpdateDTO)
        {
            await _shiftTemplateService.UpdateShiftTemplate(shiftTemplateUpdateDTO);
            return NoContent();
        }

        #endregion

        #region Update Shift Break Template 

        [HttpPut("update-shift-break-template")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateShiftBreakTemplate(ShiftBreakTemplateDTO shiftBreakTemplateDTO)
        {
            await _shiftTemplateService.UpdateShiftBreakTemplate(shiftBreakTemplateDTO);
            return NoContent();
        }

        #endregion

        #region Update Shift Template Pop Count

        [HttpPut("update-shift-template-pop-count/{shiftTemplateId}")]
        public async Task<IActionResult> UpdateShiftTemplatePopCount(int shiftTemplateId)
        {
            await _shiftTemplateService.UpdateShiftTemplatePopCount(shiftTemplateId);
            return NoContent();
        }

        #endregion

        #region Update Shift Break Template Pop Count

        [HttpPut("update-shift-break-template-pop-count/{shiftBreakTemplateId}")]
        public async Task<IActionResult> UpdateShiftBreakTemplatePopCount(int shiftBreakTemplateId)
        {
            await _shiftTemplateService.UpdateShiftBreakTemplatePopCount(shiftBreakTemplateId);
            return NoContent();
        }

        #endregion

        #region Delete Shift Template

        [HttpDelete("delete-shift-template/{shiftTemplateId}")]
        public async Task<IActionResult> DeleteShiftTemplate(int shiftTemplateId)
        {
            await _shiftTemplateService.DeleteShiftTemplate(shiftTemplateId);
            return NoContent();
        }

        #endregion

        #region Delete Shift Break Template

        [HttpDelete("delete-shift-break-template/{shiftBreakTemplateId}")]
        public async Task<IActionResult> DeleteShiftBreakTemplate(int shiftBreakTemplateId)
        {
            await _shiftTemplateService.DeleteShiftBreakTemplate(shiftBreakTemplateId);
            return NoContent();
        }

        #endregion
    }
}
