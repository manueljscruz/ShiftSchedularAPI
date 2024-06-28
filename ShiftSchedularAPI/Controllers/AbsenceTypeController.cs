using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;

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
        [ProducesResponseType(200, Type = typeof(AbsenceType))]
        public async Task<IActionResult> GetAbsenceTypeById(int id)
        {
            var absenceType = await _absenceTypeService.GetAbsenceTypeById(id);

            if (absenceType == null)
            {
                return NotFound();
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
        [ProducesResponseType(200)]
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
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllAbsenceTypesByLocalization(string lcode)
        {
            var absenceTypesLocalization = await _absenceTypeService.GetAllAbsenceTypesByLocalization(lcode);

            return Ok(absenceTypesLocalization);
        }

        #endregion

        #region Add Absence Type

        /// <summary>
        /// Add Absence Type to the database
        /// </summary>
        /// <param name="absenceType"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddAbsenceType(string strNewAbsenceType)
        {
            int newAbsenceTypeId = await _absenceTypeService.AddAbsenceType(strNewAbsenceType);

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
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateAbsenceType(AbsenceType absenceType)
        {
            await _absenceTypeService.UpdateAbsenceType(absenceType);

            return NoContent();
        }

        #endregion

        #region Delete Absence Type By Id

        /// <summary>
        /// Delete instance by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete-by-id/{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteAbsenceTypeById(int id)
        {
            await _absenceTypeService.DeleteAbsenceType(id);

            return NoContent();
        }

        #endregion

        #endregion
    }
}
