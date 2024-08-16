using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.APIManagement;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BaseEntityRuleController : ControllerBase
    {
        private readonly IBaseEntityRuleService _baseEntityRuleService;

        #region Constructor

        public BaseEntityRuleController(IBaseEntityRuleService baseEntityRuleService)
        {
            _baseEntityRuleService = baseEntityRuleService;
        }

        #endregion

        #region Methods

        #region Add Base Entity Rule

        [HttpPost("add")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddBaseEntityRule(BaseEntityRuleSubmissionModel baseEntityRuleSubmissionModel)
        {
            if (baseEntityRuleSubmissionModel == null)
            {
                return BadRequest();
            }
            int newId = await _baseEntityRuleService.AddBaseEntityRule(baseEntityRuleSubmissionModel);
            return Created($"/api/baseEntityRule/{newId}", "Base Entity Rule Added");
        }

        #endregion

        #region Add Base Entity Rule Specification

        [HttpPost("add-rule-spec")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddBaseEntityRuleSpec(BaseEntityRuleSpecificationSubmissionModel submissionModel)
        {
            if (submissionModel == null)
            {
                return BadRequest();
            }
            bool result = await _baseEntityRuleService.AddBaseEntityRuleSpecification(submissionModel);
            if (result)
                return Created($"/api/baseEntityRule/newBaseEntityRule/success","");
            else
                return StatusCode(StatusCodes.Status500InternalServerError);
        }


        #endregion

        #region Get Base Entity Rule By Id

        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(200, Type = typeof(BaseEntityRule))]
        public async Task<IActionResult> GetBaseEntityRuleById(int id)
        {
            var baseEntityRule = await _baseEntityRuleService.GetBaseEntityRuleById(id);
            if (baseEntityRule == null)
            {
                return NotFound();
            }
            return Ok(baseEntityRule);
        }

        #endregion

        #region Get All Base Entity Rules By Localization

        [HttpGet("get-all-by-localization/{lcode}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllBaseEntityRulesByLocalization(string lcode)
        {
            var baseEntityRules = _baseEntityRuleService.GetBaseEntityRules(lcode);
            return Ok(baseEntityRules);
        }

        #endregion

        #region Update Base Entity Rule

        [HttpPut("update-base-entity-rule")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateBaseEntityRule(BaseEntityRule baseEntityRule)
        {
            await _baseEntityRuleService.UpdateBaseEntityRule(baseEntityRule);
            return NoContent();
        }

        #endregion

        #region Update Base Entity Rule Specification

        [HttpPut("update-base-entity-rule-spec")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateBaseEntityRuleSpecification(BaseEntityRuleSpecification baseEntityRuleSpec)
        {
            await _baseEntityRuleService.UpdateBaseEntityRuleSpecification(baseEntityRuleSpec);
            return NoContent();
        }

        #endregion

        #endregion
    }
}
