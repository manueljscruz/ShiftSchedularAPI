using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocalizationController : ControllerBase
    {
        public readonly ILocalizationService _localizationService;

        #region Constructor

        public LocalizationController(ILocalizationService localizationService)
        {
            _localizationService = localizationService;
        }

        #endregion

        #region Methods

        #region Get Localization By Id

        /// <summary>
        /// Gets the localization instance by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(200, Type = typeof(Localization))]
        public async Task<IActionResult> GetLocalizationById(int id)
        {
            var localization = await _localizationService.GetLocalizationById(id);

            if (localization == null)
            {
                return NotFound();
            }

            return Ok(localization);
        }

        #endregion

        #region Get Localization By Language Code

        /// <summary>
        /// Searches for localization instances by the language code
        /// </summary>
        /// <param name="languageCode"></param>
        /// <returns></returns>
        [HttpGet("get-by-code/{languageCode}")]
        [ProducesResponseType(200, Type = typeof(Localization))]
        public async Task<IActionResult> GetLocalizationByLanguageCode(string languageCode)
        {
            var localization = await _localizationService.GetLocalizationByLanguageCode(languageCode);

            if (localization == null)
            {
                return NotFound();
            }

            return Ok(localization);
        }

        #endregion

        #region Get All Localizations

        /// <summary>
        /// Gets all localization instances
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllGenders()
        {
            var localizations = await _localizationService.GetAllLocalizations();

            return Ok(localizations);
        }

        #endregion

        #region Add Localization

        /// <summary>
        /// Adds a new localization instance to the database
        /// </summary>
        /// <param name="strNewLocalizationCode"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddLocalization(string strNewLocalizationCode)
        {
            int newLocalizationId = await _localizationService.AddLocalization(strNewLocalizationCode);

            return Created($"/api/localization/{newLocalizationId}", "Localization Added");
        }

        #endregion

        #region Update Localization

        /// <summary>
        /// Updates a localization instance in the db
        /// </summary>
        /// <param name="localizationInstance"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateLocalization(Localization localizationInstance)
        {
            await _localizationService.UpdateLocalization(localizationInstance);

            return NoContent();
        }

        #endregion

        #region Delete Localization by Id

        /// <summary>
        /// Deletes a localization instance by an identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete-by-id/{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteLocalizationById(int id)
        {
            await _localizationService.DeleteLocalization(id);

            return NoContent();
        }

        #endregion

        #endregion
    }
}
