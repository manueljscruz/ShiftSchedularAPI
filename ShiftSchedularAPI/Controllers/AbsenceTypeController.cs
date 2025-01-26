using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularRL.Resources.AbsenceManagement;
using ShiftSchedularRL.Resources.Shared;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AbsenceTypeController : ControllerBase
    {
        private readonly IAbsenceTypeService _absenceTypeService;

        #region Constructor

        public AbsenceTypeController(IAbsenceTypeService absenceTypeService)
        {
            _absenceTypeService = absenceTypeService;
        }

        #endregion

        #region Methods

        #region Get Absence Type By Id 

        /// <summary>
        /// Gets Absence Type instance by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAbsenceTypeById(int id)
        {
            var absenceType = await _absenceTypeService.GetAbsenceTypeById(id);

            if (absenceType == null)
            {
                return NotFound(AbsenceRelatedMessages.AbsenceNotFound);
            }

            return Ok(absenceType);
        }

        #endregion

        #region Get All Absences Types

        /// <summary>
        /// Get all absences type records
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllAbsenceTypes()
        {
            var absenceTypes = await _absenceTypeService.GetAllAbsenceTypes();
            return Ok(absenceTypes);
        }

        #endregion

        #region Get Absence Types By Localization

        /// <summary>
        /// Get all absence type records by localization code
        /// </summary>
        /// <param name="lcode"></param>
        /// <returns></returns>
        [HttpGet("get-all-absence-types-by-localization/{lcode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetAllAbsenceTypesByLocalization(string lcode)
        {
            if(string.IsNullOrEmpty(lcode))
            {
                return BadRequest(SharedMessages.LocalizationEmpty);
            }

            var absenceTypesLocalization = await _absenceTypeService.GetAllAbsenceTypesByLocalization(lcode);

            if(absenceTypesLocalization == null || !absenceTypesLocalization.Any())
            {
                return NotFound(AbsenceRelatedMessages.AbsenceTypesNotFoundByLocal);
            }

            return Ok(absenceTypesLocalization);
        }

        #endregion

        #region Add Absence Type

        /// <summary>
        /// Add Absence Type to the database
        /// </summary>
        /// <param name="strNewAbsenceType"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddAbsenceType(string strNewAbsenceType)
        {
            if(string.IsNullOrEmpty(strNewAbsenceType))
            {
                return BadRequest(AbsenceRelatedMessages.NewAbsenceTypeEmpty);
            }

            int newAbsenceTypeId = await _absenceTypeService.AddAbsenceType(strNewAbsenceType);
            if(newAbsenceTypeId == 0)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, AbsenceRelatedMessages.NewAbsenceTypeError);
            }

            return Created($"/api/absenceType/{newAbsenceTypeId}", "Absence Type Added");
        }

        #endregion

        #region Update Absence Type

        /// <summary>
        /// Updates the absence type instance
        /// </summary>
        /// <param name="absenceType"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateAbsenceType(AbsenceType absenceType)
        {
            if(absenceType == null)
            {
                return BadRequest(AbsenceRelatedMessages.AbsenceTypeIsInvalid);
            }

            if(await _absenceTypeService.UpdateAbsenceType(absenceType))
                return NoContent();
            else
                return StatusCode(StatusCodes.Status500InternalServerError, SharedMessages.UnexpectedError);
        }

        #endregion

        #region Delete Absence Type By Id

        /// <summary>
        /// Delete instance by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete-by-id/{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAbsenceTypeById(int id)
        {
            if(id <= 0)
            {
                return BadRequest(AbsenceRelatedMessages.AbsenceTypeIdRequired);
            }

            if(await _absenceTypeService.DeleteAbsenceType(id))
                return NoContent();

            else
                return StatusCode(StatusCodes.Status500InternalServerError, SharedMessages.UnexpectedError);
        }

        #endregion

        #endregion
    }
}
