using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using System.Text.Json.Serialization;
using System.Text.Json;

namespace ShiftSchedularAPI.Controllers
{
    [ApiController]
    [Route("gender")]
    public class GenderController : ControllerBase
    {
        public readonly IGenderService _genderService;

        #region Constructor

        public GenderController(IGenderService genderService)
        {
            _genderService = genderService;
        }

        #endregion

        #region Methods

        #region Get Gender By Id

        /// <summary>
        /// Gets the Gender instance by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns></returns>
        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(200, Type = typeof(Gender))]
        public async Task<IActionResult> GetGenderById(int id)
        {
            var gender = await _genderService.GetGenderById(id);

            if (gender == null)
            {
                return NotFound();
            }

            return Ok(gender);
        }

        #endregion

        #region Get All Genders

        /// <summary>
        /// Get all genders records
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllGenders()
        {
            var genders = await _genderService.GetAllGenders();

            return Ok(genders);
        }

        #endregion

        #region Get Genders By Localization

        /// <summary>
        /// Get all genders records by localization code
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all-genders-by-localization/{lcode}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllGendersByLocalization(string lcode)
        {
            var gendersLocalization = await _genderService.GetAllGendersByLocalization(lcode);

            return Ok(gendersLocalization);
        }

        #endregion

        #region Add Gender

        /// <summary>
        /// Adds a new Gender to the database
        /// </summary>
        /// <param name="strNewGenderValue"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddGender(string strNewGenderValue)
        {
            int newGenderId = await _genderService.AddGender(strNewGenderValue);

            return Created($"/api/gender/{newGenderId}", "Gender Added");
        }

        #endregion

        #region Add Gender Localization

        /// <summary>
        /// Adds a new gender localization entry
        /// </summary>
        /// <param name="genderLocalizationSubmission"></param>
        /// <returns></returns>
        [HttpPost("add-gender-localization")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddGenderLocalization(GenderLocalizationSubmissionModel genderLocalizationSubmission)
        {
            bool result = await _genderService.AddGenderLocalization(genderLocalizationSubmission);

            if(result)
                return Created($"/api/gender/add-gender-localization", "Gender Localization entry submitted");
            else 
                return BadRequest("Error occured when creating a gender localization entry");
        }

        #endregion

        #region Update Gender

        /// <summary>
        /// Updates the gender instance
        /// </summary>
        /// <param name="genderInstance"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateGender(Gender genderInstance)
        {
            await _genderService.UpdateGender(genderInstance);

            return NoContent();
        }

        #endregion

        #region Delete Gender by Id

        /// <summary>
        /// Delete instance by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns></returns>
        [HttpDelete("delete-by-id/{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteGenderById(int id)
        {
            await _genderService.DeleteGender(id);

            return NoContent();
        }

        #endregion

        #endregion
    }
}
