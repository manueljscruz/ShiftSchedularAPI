using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularRL.Resources.Dashboard;
using ShiftSchedularRL.Resources.Home;
using System.Web;

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

        /// <summary>
        /// Get entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntityById(string id)
        {
            if (!string.IsNullOrEmpty(id))
            {
                string decodedEntityId = HttpUtility.UrlDecode(id);
                var entity = await _entityService.GetEntityById(id);
                if (entity == null)
                {
                    return NotFound(EntitiesRelatedMessages.UpdateEntityNotFound);
                }
                return Ok(entity);
            }
            else
            {
                return BadRequest(EntitiesRelatedMessages.DeleteEntityNoIdentifierError);
            }
        }

        #endregion

        #region Get All Entities

        /// <summary>
        /// Gets all the work entities
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllEntities()
        {
            var entities = await _entityService.GetAllEntities();

            return Ok(entities);
        }

        #endregion

        #region Get Entities by Worker ID

        /// <summary>
        /// Gets Work Entities that the worker is associated with
        /// </summary>
        /// <param name="workerId">Identifier of the worker</param>
        /// <returns></returns>
        [HttpGet("get-entities-by-worker-id/{workerId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntitiesByWorkerId(string workerId)
        {
            if(string.IsNullOrEmpty(workerId))
            {
                return BadRequest(WorkerRelatedMessages.WorkerIdentifierIsEmpty);
            }
            var entities = await _entityService.GetEntitiesByWorkerId(workerId);
            return Ok(entities);
        }

        #endregion

        #region Get Entities Members View Model 

        /// <summary>
        /// Gets the members of the entity in a view model
        /// </summary>
        /// <param name="entityId">Identifier of the entity</param>
        /// <param name="lcode">Language Code</param>
        /// <returns></returns>
        [HttpGet("get-entities-members-view-model/{entityId}/{lcode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntitiesMembersViewModel(string entityId, string lcode)
        {
            if (!string.IsNullOrEmpty(entityId))
            {
                string decodedEntityId = HttpUtility.UrlDecode(entityId);
                var entities = await _entityService.GetEntitiesMembersViewModel(decodedEntityId, lcode);
                return Ok(entities);
            }
            else
            {
                return BadRequest(EntitiesRelatedMessages.DeleteEntityNoIdentifierError);
            }
        }

        #endregion

        #region Get Entities Skills

        /// <summary>
        /// Gets the skills of the entity
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="lcode"></param>
        /// <returns></returns>
        [HttpGet("get-entity-skills/{entityId}/{lcode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntitiesSkills(string entityId, string lcode)
        {
            if (!string.IsNullOrEmpty(entityId))
            {
                string decodedEntityId = HttpUtility.UrlDecode(entityId);
                var skills = await _entityService.GetEntitySkills(entityId, lcode);
                return Ok(skills);
            }
            else
            {
                return BadRequest(EntitiesRelatedMessages.DeleteEntityNoIdentifierError);
            }
        }

        #endregion

        #region Get Entity Profile View Model

        [HttpPost("get-entity-profile-view-model")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntityProfileViewModel(EntityProfileViewModelRequestDTO profileViewModelRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            profileViewModelRequest.EntityId = HttpUtility.UrlDecode(profileViewModelRequest.EntityId);
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

        #region Add New Entity Member

        [HttpPost("add-new-entity-member")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AddNewEntityMember(AddNewMemberDTO newMemberDTO)
        {
            BaseResponse<object> response = await _entityService.AddNewEntityMember(newMemberDTO);

            return Ok(response);
        }

        #endregion

        [HttpPut("update-entity-member")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> UpdateEntityMember(EditMemberDTO updateEntityMemberDTO)
        {
            BaseResponse<bool> response = await _entityService.UpdateEntityMember(updateEntityMemberDTO);
            return Ok(response);
        }

        #endregion
    }
}
