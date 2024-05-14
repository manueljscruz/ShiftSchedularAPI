using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.APIManagement;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShiftBreakTypeController : ControllerBase
    {
        private readonly IShiftBreakTypeService _shiftBreakTypeService;

        #region Constructor

        public ShiftBreakTypeController(IShiftBreakTypeService shiftBreakTypeService)
        {
            _shiftBreakTypeService = shiftBreakTypeService;
        }

        #endregion

        #region Methods

        #region Get Shift Break Type By Id 

        /// <summary>
        /// Gets Shift Break Type instance by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(200, Type = typeof(ShiftBreakType))]
        public async Task<IActionResult> GetShiftBreakTypeById(int id)
        {
            var entityType = await _shiftBreakTypeService.GetShiftBreakTypeById(id);

            if (entityType == null)
            {
                return NotFound();
            }

            return Ok(entityType);
        }

        #endregion

        #region Get All Shift Break Types

        /// <summary>
        /// Get all shift break type records
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllShiftBreakTypes()
        {
            var entityTypes = await _shiftBreakTypeService.GetAllShiftBreakTypes();

            return Ok(entityTypes);
        }

        #endregion

        #region Get All Shift Break Types By Localization

        /// <summary>
        /// Get all shift break type records by localization code
        /// </summary>
        /// <param name="lcode"></param>
        /// <returns></returns>
        [HttpGet("get-all-shift-break-types-by-localization/{lcode}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllShiftBreakTypesByLocalization(string lcode)
        {
            var entityTypesLocalization = await _shiftBreakTypeService.GetAllShiftBreakTypesByLocalization(lcode);

            return Ok(entityTypesLocalization);
        }

        #endregion

        #region Add Shift Break Type

        /// <summary>
        /// Add Shift break Type to the database
        /// </summary>
        /// <param name="strNewShiftBreakType"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddShiftBreakType(string strNewShiftBreakType)
        {
            int newShiftBreakTypeId = await _shiftBreakTypeService.AddShiftBreakType(strNewShiftBreakType);

            return Created($"/api/shiftBreakType/{newShiftBreakTypeId}", "Shift Break Type Added");
        }

        #endregion

        #region Add Shift Break Type Localization

        [HttpPost("add-localized")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddShiftBreakTypeLocalized(ShiftBreakTypeLocalizationSubmissionModel submissionModel)
        {
            bool localizedCreated = await _shiftBreakTypeService.AddShiftBreakTypeLocalization(submissionModel);
            if (localizedCreated)
                return Created($"/api/shiftBreakType/localized-added", "Shift Break Type Added");
            else
                return BadRequest("An error has occured while adding a localized entry");
        }

        #endregion

        #region Update Shift Break Type

        /// <summary>
        /// Updates the shift break type instance
        /// </summary>
        /// <param name="shiftBreakType"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateEntityType(ShiftBreakType shiftBreakType)
        {
            await _shiftBreakTypeService.UpdateShiftBreakType(shiftBreakType);

            return NoContent();
        }

        #endregion

        #region Delete Shift Break Type By Id

        /// <summary>
        /// Delete instance by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete-by-id/{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteShiftBreakTypeById(int id)
        {
            await _shiftBreakTypeService.DeleteShiftBreakType(id);

            return NoContent();
        }

        #endregion

        #endregion
    }
}
