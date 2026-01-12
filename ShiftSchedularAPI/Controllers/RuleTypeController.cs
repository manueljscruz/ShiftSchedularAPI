using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.APIManagement;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("general")]
    public class RuleTypeController : ControllerBase
    {
        private readonly IRuleTypeService _ruleTypeService;

        public RuleTypeController(IRuleTypeService ruleTypeService)
        {
            _ruleTypeService = ruleTypeService;
        }

        #region Methods

        #region Get Rule Type By Id

        /// <summary>
        /// Gets the Rule type instance by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns></returns>
        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(200, Type = typeof(RuleType))]
        public async Task<IActionResult> GetRuleTypeById(int id)
        {
            var ruleType = await _ruleTypeService.GetRuleTypeById(id);

            if (ruleType == null)
            {
                return NotFound();
            }

            return Ok(ruleType);
        }

        #endregion

        #region Get All Rule Types

        /// <summary>
        /// Get all rule type records
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllRuleTypes()
        {
            var ruleTypes = await _ruleTypeService.GetAllRuleTypes();

            return Ok(ruleTypes);
        }

        #endregion

        #region Get Rule Types By Localization

        /// <summary>
        /// Get all genders records by localization code
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all-rule-types-by-localization/{lcode}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllRuleTypesByLocalization(string lcode)
        {
            var ruleTypeLocalizations = await _ruleTypeService.GetAllRuleTypesByLocalization(lcode);

            return Ok(ruleTypeLocalizations);
        }

        #endregion

        #region Add Rule Type

        /// <summary>
        /// Adds a new rule type to the database
        /// </summary>
        /// <param name="strNewRuleType"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddRuleType(AddRuleTypeDTO newRuleType)
        {
            int newRuleTypeId = await _ruleTypeService.AddRuleType(newRuleType);

            return Created($"/api/ruletype/{newRuleTypeId}", "Rule Type Added");
        }

        #endregion

        #region Add Rule Type Localization

        /// <summary>
        /// Adds a new rule type localization entry
        /// </summary>
        /// <param name="ruleTypeLocalizationSubmissionModel"></param>
        /// <returns></returns>
        [HttpPost("add-rule-type-localization")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddRuleTypeLocalization(RuleTypeLocalizationSubmissionModel ruleTypeLocalizationSubmissionModel)
        {
            bool result = await _ruleTypeService.AddRuleTypeLocalization(ruleTypeLocalizationSubmissionModel);

            if (result)
                return Created($"/api/ruletype/add-rule-type-localization", "Rule Type Localization entry submitted");
            else
                return BadRequest("Error occured when creating a rule type localization entry");
        }

        #endregion

        #region Update Rule Type

        /// <summary>
        /// Updates the rule type instance
        /// </summary>
        /// <param name="ruleType"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateRuleType(RuleType ruleType)
        {
            await _ruleTypeService.UpdateRuleType(ruleType);

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
        public async Task<IActionResult> DeleteRuleTypeById(int id)
        {
            await _ruleTypeService.DeleteRuleTypeById(id);

            return NoContent();
        }

        #endregion


        #endregion
    }
}
