using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;

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
