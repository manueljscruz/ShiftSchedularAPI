using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    [EnableRateLimiting("general")]
    public class EntityRuleController : ControllerBase
    {
        private readonly IEntityRuleService _entityRuleService;

        #region Constructor

        public EntityRuleController(IEntityRuleService entityRuleService)
        {
            _entityRuleService = entityRuleService;
        }

        #endregion

        #region Methods

        #region Get Entity Rule By Id

        [Authorize(Roles = "Admin")]
        [HttpGet("get-by-id/{id}/{lcode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntityRuleById(Guid id, string lcode)
        {
            if(id == Guid.Empty || string.IsNullOrEmpty(lcode))
            {
                return BadRequest();
            }

            // string entityRuleId = HttpUtility.UrlDecode(id);

            var entityRule = await _entityRuleService.GetEntityRuleById(id, lcode);
            return Ok(entityRule);
        }

        #endregion

        #region Get Entity Rules View Model

        [HttpPost("get-entity-rules-view-model")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntityRulesViewModel(BaseViewModelRequest viewModelRequest)
        {
            if(viewModelRequest == null)
            {
                return BadRequest();
            }

            // viewModelRequest.EntityId = HttpUtility.UrlDecode(viewModelRequest.EntityId);

            var viewModel = await _entityRuleService.GetEntityRuleViewModel(viewModelRequest);
            return Ok(viewModel);
        }

        #endregion

        #region Add Entity Rule

        [HttpPost("add-entity-rule")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddEntityRule([FromBody] AddEntityRuleDTO entityRuleDTO)
        {
            if (entityRuleDTO == null)
            {
                return BadRequest();
            }

            // entityRuleDTO.EntityId = HttpUtility.UrlDecode(entityRuleDTO.EntityId);

            var entityRule = await _entityRuleService.AddEntityRule(entityRuleDTO);
            return Ok(entityRule);
        }

        #endregion

        #region Add Entity Rule Specification

        [HttpPost("add-entity-rule-spec")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddEntityRuleSpec(AddEntityRuleSpecificationDTO addEntityRuleSpec)
        {
            if(addEntityRuleSpec == null)
            {
                return BadRequest();
            }

            var entityRuleSpec = await _entityRuleService.AddEntityRuleSpecification(addEntityRuleSpec);
            return Ok(entityRuleSpec);
        }

        #endregion

        #region Update Entity Rule

        [HttpPut("update-entity-rule")]
        [ProducesResponseType(204)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEntityShift([FromBody] EntityRuleDTO entityRule)
        {
            if(entityRule == null)
            {
                return BadRequest();
            }
            // entityRule.EntityRuleId = HttpUtility.UrlDecode(entityRule.EntityRuleId);
            var updatedEntityRule = await _entityRuleService.UpdateEntityRule(entityRule);
            return Ok(updatedEntityRule);
        }

        #endregion

        #region Update Entity Rule Spec

        [HttpPut("update-entity-rule-spec")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEntityShiftBreak([FromBody] EntityRuleSpecificationDTO entityRuleSpec)
        {
            if(entityRuleSpec == null)
            {
                return BadRequest();
            }

            var updatedEntityRuleSpec = await _entityRuleService.UpdateEntityRuleSpecification(entityRuleSpec);
            return Ok(updatedEntityRuleSpec);
        }

        #endregion

        #region Delete Entity Rule

        [HttpDelete("delete-entity-rule")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEntityRule(DeleteEntityObjectDTO deleteEntityObjectDTO)
        {
            if(deleteEntityObjectDTO.EntityId == Guid.Empty || deleteEntityObjectDTO.ObjectId == Guid.Empty)
            {
                return BadRequest();
            }

            var response = await _entityRuleService.DeleteEntityRule(deleteEntityObjectDTO.EntityId, deleteEntityObjectDTO.ObjectId);
            if (response.Success)
            {
                return Ok(response);
            }
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        }

        #endregion

        #region Delete Entity Rule Specification

        [HttpDelete("delete-entity-rule-spec")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEntityRuleSpec(DeleteEntityRuleSpecDTO deleteEntityRuleSpec)
        {
            if(deleteEntityRuleSpec.EntityId == Guid.Empty || deleteEntityRuleSpec.ObjectId == Guid.Empty || deleteEntityRuleSpec.RuleSpecId == 0)
            {
                return BadRequest();
            }

            var response = await _entityRuleService.DeleteEntityRuleSpecification(deleteEntityRuleSpec.ObjectId, deleteEntityRuleSpec.RuleSpecId);
            return Ok(response);
        }

        #endregion

        #region Delete Rule Specifications

        [HttpDelete("delete-entity-rule-specs")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> DeleteEntityRuleSpecs(Guid entityRuleId)
        {
            if(entityRuleId == Guid.Empty)
            {
                return BadRequest();
            }

            // entityRuleId = HttpUtility.UrlDecode(entityRuleId);

            var response = await _entityRuleService.DeleteEntityRuleSpecifications(entityRuleId);
            return Ok(response);
        }

        #endregion

        #endregion
    }
}
