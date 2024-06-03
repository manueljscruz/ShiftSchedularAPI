using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.APIManagement;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BusinessAspectController : ControllerBase
    {
        private readonly IBusinessAspectService _businessAspectService;

        #region Constructor

        public BusinessAspectController(IBusinessAspectService businessAspectService)
        {
            _businessAspectService = businessAspectService;
        }

        #endregion

        #region Methods

        #region Get Business Aspect By Id

        /// <summary>
        /// Gets the Business Aspect instance by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns></returns>
        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(200, Type = typeof(BusinessAspect))]
        public async Task<IActionResult> GetBusinessAspectById(int id)
        {
            var businessAspect = await _businessAspectService.GetBusinessAspectById(id);

            if (businessAspect == null)
            {
                return NotFound();
            }

            return Ok(businessAspect);
        }

        #endregion

        #region Get All Business Aspects

        /// <summary>
        /// Get all business aspects records
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllBusinessAspects()
        {
            var businessAspect = await _businessAspectService.GetAllBusinessAspects();

            return Ok(businessAspect);
        }

        #endregion

        #region Get Business Aspects By Localization

        /// <summary>
        /// Get all business aspect records by localization code
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all-business-aspects-by-localization/{lcode}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllBusinessAspectsByLocalization(string lcode)
        {
            var businessAspectLocalizations = await _businessAspectService.GetAllBusinessAspectsByLocalization(lcode);

            return Ok(businessAspectLocalizations);
        }

        #endregion

        #region Add Business Aspect

        /// <summary>
        /// Adds a new business aspect to the database
        /// </summary>
        /// <param name="strNewBusinessASpect"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddBusinessAspect(string strNewBusinessASpect)
        {
            int businessAspectId = await _businessAspectService.AddBusinessAspect(strNewBusinessASpect);

            return Created($"/api/businessaspect/{businessAspectId}", "Business Aspect Added");
        }

        #endregion

        #region Add Business Aspect Localization

        /// <summary>
        /// Adds a new business aspect localization entry
        /// </summary>
        /// <param name="ruleTypeLocalizationSubmissionModel"></param>
        /// <returns></returns>
        [HttpPost("add-business-aspect-localization")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddBusinessAspectLocalization(BusinessAspectLocalizationSubmissionModel submissionModel)
        {
            bool result = await _businessAspectService.AddBusinessAspectLocalization(submissionModel);

            if (result)
                return Created($"/api/businessaspect/add-business-aspect-localization", "Business Aspect Localization entry submitted");
            else
                return BadRequest("Error occured when creating a rule type localization entry");
        }

        #endregion

        #region Update Business Aspect

        /// <summary>
        /// Updates the business aspect instance
        /// </summary>
        /// <param name="businessAspect"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateRuleType(BusinessAspect businessAspect)
        {
            await _businessAspectService.UpdateBusinessAspect(businessAspect);

            return NoContent();
        }

        #endregion

        #region Delete Rule Type by Id

        /// <summary>
        /// Delete instance by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns></returns>
        [HttpDelete("delete-by-id/{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteBusinessAspectById(int id)
        {
            await _businessAspectService.DeleteBusinessAspectById(id);

            return NoContent();
        }

        #endregion

        #endregion

    }
}
