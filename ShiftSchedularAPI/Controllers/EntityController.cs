using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntityController : ControllerBase
    {
        private readonly IEntityService _entityService;

        #region Constructor

        public EntityController(IEntityService entityService)
        {
            _entityService = entityService;
        }

        #endregion

        #region Methods

        #region Get Entity By Id

        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(200, Type = typeof(Entity))]
        public async Task<IActionResult> GetEntityById(string id)
        {
            var entity = await _entityService.GetEntityById(id);
            if (entity == null)
            {
                return NotFound();
            }
            return Ok(entity);
        }

        #endregion

        #region Get All Entities

        [HttpGet("get-all")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllEntities()
        {
            var entities = await _entityService.GetAllEntities();

            return Ok(entities);
        }

        #endregion

        #region Get Entities by Worker ID

        [HttpGet("get-entities-by-worker-id/{workerId}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetEntitiesByWorkerId(string workerId)
        {
            var entities = await _entityService.GetEntitiesByWorkerId(workerId);
            return Ok(entities);
        }

        #endregion

        #region Get Entities Members View Model 

        [HttpGet("get-entities-members-view-model/{entityId}/{lcode}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetEntitiesMembersViewModel(string entityId, string lcode)
        {
            var entities = await _entityService.GetEntitiesMembersViewModel(entityId, lcode);
            return Ok(entities);
        }

        #endregion

        #region Get Entity Profile View Model

        [HttpPost("get-entity-profile-view-model")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetEntityProfileViewModel(EntityProfileViewModelRequestDTO profileViewModelRequest)
        {
            var entityVM = await _entityService.GetEntityProfileViewModel(profileViewModelRequest);
            return Ok(entityVM);
        }

        #endregion

        #region Add Entity

        [HttpPost("add")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddEntity(FormEntityDTO newEntity)
        {
            BaseResponse<Entity> response = await _entityService.AddEntity(newEntity);

            return Ok(response);
        }

        #endregion

        #region Update Entity

        [HttpPut("update")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateEntity(FormEntityDTO entity)
        {
            BaseResponse<bool> response = await _entityService.UpdateEntity(entity);
            return Ok(response);
        }

        #endregion

        #region Delete Entity by Id

        [HttpDelete("delete-by-id/{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteEntityById([FromRoute]string id)
        {
            BaseResponse<bool> response = await _entityService.DeleteEntityById(id);

            return Ok(response);

        }

        #endregion

        [HttpPost("add-new-entity-member")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AddNewEntityMember(AddNewMemberDTO newMemberDTO)
        {
            BaseResponse<object> response = await _entityService.AddNewEntityMember(newMemberDTO);

            return Ok(response);
        }

        #endregion
    }
}
