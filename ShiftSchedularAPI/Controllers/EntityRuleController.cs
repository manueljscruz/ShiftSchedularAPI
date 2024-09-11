using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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

        [HttpGet("get-by-id/{id}/{lcode}")]
        [ProducesResponseType(200, Type = typeof(Shift))]
        public async Task<IActionResult> GetEntityRuleById(string id, string lcode)
        {
            var entityRule = await _entityRuleService.GetEntityRuleById(id, lcode);
            return Ok(entityRule);
        }

        #endregion

        #region Get Entity Rules View Model

        [HttpPost("get-entity-rules-view-model")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetEntityRulesViewModel(BaseViewModelRequest viewModelRequest)
        {
            var viewModel = await _entityRuleService.GetEntityRuleViewModel(viewModelRequest);
            return Ok(viewModel);
        }

        #endregion

        #region Add Entity Rule

        [HttpPost("add-entity-rule")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AddEntityRule([FromBody] AddEntityRuleDTO entityRuleDTO)
        {
            var entityRule = await _entityRuleService.AddEntityRule(entityRuleDTO);
            return Ok(entityRule);
        }

        #endregion

        #region Add Entity Rule Specification

        [HttpPost("add-entity-rule-spec")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddEntityRuleSpec(AddEntityRuleSpecificationDTO addEntityRuleSpec)
        {
            var entityRuleSpec = await _entityRuleService.AddEntityRuleSpecification(addEntityRuleSpec);
            return Ok(entityRuleSpec);
        }

        #endregion

        #region Update Entity Rule

        [HttpPut("update-entity-rule")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateEntityShift([FromBody] EntityRuleDTO entityRule)
        {
            var updatedEntityRule = await _entityRuleService.UpdateEntityRule(entityRule);
            return Ok(updatedEntityRule);
        }

        #endregion

        #region Update Entity Rule Spec

        [HttpPut("update-entity-rule-spec")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateEntityShiftBreak([FromBody] EntityRuleSpecificationDTO entityRuleSpec)
        {
            var updatedEntityRuleSpec = await _entityRuleService.UpdateEntityRuleSpecification(entityRuleSpec);
            return Ok(updatedEntityRuleSpec);
        }

        #endregion

        #region Delete Shift

        [HttpDelete("delete-entity-rule/{entityId}/{entityRuleId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteEntityRule(string entityId, string entityRuleId)
        {
            var response = await _entityRuleService.DeleteEntityRule(entityId, entityRuleId);
            return Ok(response);
        }

        #endregion

        #region Delete Shift Break

        [HttpDelete("delete-entity-rule-spec/{entityRuleId}/{specId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteEntityRuleSpec(string entityRuleId, int specId)
        {
            var response = await _entityRuleService.DeleteEntityRuleSpecification(entityRuleId, specId);
            return Ok(response);
        }

        #endregion

        #region Delete Rule Specifications

        [HttpDelete("delete-entity-rule-specs/{entityRuleId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteEntityRuleSpecs(string entityRuleId)
        {
            var response = await _entityRuleService.DeleteEntityRuleSpecifications(entityRuleId);
            return Ok(response);
        }

        #endregion

        #endregion
    }
}
