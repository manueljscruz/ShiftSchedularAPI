using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
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
    [EnableRateLimiting("general")]
    public class EntityController : ControllerBase
    {
        private readonly IEntityService _entityService;
        private readonly IEntityDashboardService _entityDashboardService;

        #region Constructor

        public EntityController(IEntityService entityService,
            IEntityDashboardService entityDashboardService)
        {
            _entityService = entityService;
            _entityDashboardService = entityDashboardService;
        }

        #endregion

        #region Methods

        #region Get Entity By Id

        /// <summary>
        /// Get entity by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
        [HttpGet("get-by-id/{id}/{languageCode}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntityById(Guid id, string languageCode)
        {
            if (id != Guid.Empty)
            {
                // id = HttpUtility.UrlDecode(id);
                var entity = await _entityService.GetEntityById(id, languageCode);
                if (entity == null)
                {
                    return NotFound(EntitiesRelatedMessages.EntityNotFound);
                }
                return Ok(entity);
            }
            else
            {
                return BadRequest(EntitiesRelatedMessages.EntityNoIdentifierError);
            }
        }

        #endregion

        #region Get All Entities

        /// <summary>
        /// Gets all the work entities
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Admin")]
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
        [Authorize]
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

        #region Get Entity Dashboard View Model

        [Authorize]
        [HttpPost("get-entity-dashboard-view-model")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntityDashboardViewModel(BaseViewModelRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var entityDashboardViewModel = await _entityDashboardService.GetEntityDashboardViewModel(request);
            return Ok(entityDashboardViewModel);
        }

        #endregion

        #region Get Entities Members View Model 

        /// <summary>
        /// Gets the members of the entity in a view model
        /// </summary>
        /// <param name="entityId">Identifier of the entity</param>
        /// <param name="lcode">Language Code</param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("get-entities-members-view-model")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntitiesMembersViewModel(MemberPagedModelRequestDTO memberListModelRequest)
        {
            if (memberListModelRequest.EntityId != Guid.Empty)
            {
                var entities = await _entityService.GetEntitiesMembersViewModel(memberListModelRequest);
                return Ok(entities);
            }
            else
            {
                return BadRequest(EntitiesRelatedMessages.EntityNoIdentifierError);
            }
        }

        #endregion

        #region Get Entity Members Pagination

        [Authorize]
        [HttpPost("get-entity-members-pagination")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntityMembersPagination(MemberPagedModelRequestDTO memberListModelRequest)
        {
            if(memberListModelRequest == null)
            {
                return BadRequest();
            }

            PagedList<EntityWorkerMemberDTO> memberList = await _entityService.GetEntityMembers(memberListModelRequest.EntityId, new List<string>(), memberListModelRequest.MemberFilters, memberListModelRequest.NextPage, memberListModelRequest.ItemsPerPage);

            return Ok(memberList);
        }

        #endregion

        #region Get Entities Skills

        /// <summary>
        /// Gets the skills of the entity
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="lcode"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("get-entity-skills")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntitiesSkills(BaseViewModelRequest baseViewModelRequest)
        {
            if (baseViewModelRequest != null)
            {
                // string decodedEntityId = HttpUtility.UrlDecode(entityId);
                var skills = await _entityService.GetEntitySkills(baseViewModelRequest);
                return Ok(skills);
            }
            else
            {
                return BadRequest(EntitiesRelatedMessages.EntityNoIdentifierError);
            }
        }

        #endregion

        #region Get Entity Profile View Model

        /// <summary>
        /// Gets the entity profile view model
        /// </summary>
        /// <param name="profileViewModelRequest"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("get-entity-profile-view-model")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntityProfileViewModel(BaseViewModelRequest baseViewModelRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            // profileViewModelRequest.EntityId = HttpUtility.UrlDecode(profileViewModelRequest.EntityId);
            var entityVM = await _entityService.GetEntityProfileViewModel(baseViewModelRequest);

            return Ok(entityVM);
        }

        #endregion

        #region Add Entity

        /// <summary>
        /// Adds a new entity
        /// </summary>
        /// <param name="newEntity"></param>
        /// <returns></returns>
        [Authorize]
        [HttpPost("add")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddEntity(FormEntityDTO newEntity)
        {
            if(newEntity == null)
            {
                return BadRequest();
            }

            BaseResponse<Entity> response = await _entityService.AddEntity(newEntity);

            if (!response.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            }

            return Ok(response);
        }

        #endregion

        #region Update Entity

        [Authorize]
        [HttpPut("update")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEntity(FormEntityDTO entity)
        {
            if(entity == null)
            {
                return BadRequest();
            }

            // entity.EntityId = HttpUtility.UrlDecode(entity.EntityId);
            BaseResponse<bool> response = await _entityService.UpdateEntity(entity);

            if(!response.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            }

            return Ok(response);
        }

        #endregion

        #region Delete Entity by Id

        [Authorize]
        [HttpDelete("delete-by-id")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEntityById(SingleIdentifierDTO singleIdentifierDTO)
        {
            if(singleIdentifierDTO.Identifier == Guid.Empty)
            {
                return BadRequest();
            }

            // string entityId = HttpUtility.UrlDecode(id);
            BaseResponse<bool> response = await _entityService.DeleteEntityById(singleIdentifierDTO.Identifier);

            if (!response.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            }

            return Ok(response);

        }

        #endregion

        #region Add New Entity Member

        [Authorize]
        [HttpPost("add-new-entity-member")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddNewEntityMember(AddNewMemberDTO newMemberDTO)
        {
            if(newMemberDTO == null)
            {
                return BadRequest();
            }

            BaseResponse<object> response = await _entityService.AddNewEntityMember(newMemberDTO);

            if(!response.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            }

            return Ok(response);
        }

        #endregion

        #region Update Entity Member

        [Authorize]
        [HttpPut("update-entity-member")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateEntityMember(EditMemberDTO updateEntityMemberDTO)
        {
            if(updateEntityMemberDTO == null)
            {
                return BadRequest();
            }

            BaseResponse<bool> response = await _entityService.UpdateEntityMember(updateEntityMemberDTO);

            if(!response.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            }

            return Ok(response);
        }

        #endregion

        #region Delete Entity Member

        [Authorize]
        [HttpDelete("delete-entity-member")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEntityMember(DeleteMemberDTO workerData)
        {
            if (workerData == null)
            {
                return BadRequest();
            }

            BaseResponse<bool> response = await _entityService.DeleteEntityMember(workerData);
            if (!response.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            }

            return NoContent();
        }

        #endregion

        #region Get Child Entities

        /// <summary>
        /// Gets all direct child entities of a given parent entity
        /// </summary>
        /// <param name="parentId">Identifier of the parent entity</param>
        /// <returns>List of child entities</returns>
        [Authorize]
        [HttpGet("get-children/{parentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetChildEntities(Guid parentId)
        {
            if (parentId == Guid.Empty)
            {
                return BadRequest(EntitiesRelatedMessages.EntityNoIdentifierError);
            }

            List<EntityDTO> children = await _entityService.GetChildEntities(parentId);
            return Ok(children);
        }

        #endregion

        #region Convert Bot To User

        [Authorize]
        [HttpPost("convert-bot-to-user")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ConvertBotToUser([FromBody] ConvertBotToUserDTO convertBotToUserDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BaseResponse<EntityWorkerMemberDTO> conversionResponse = await _entityService.ConvertBotToUser(convertBotToUserDTO);

            if (conversionResponse.Success)
                return Ok(conversionResponse);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, conversionResponse.Message);
        }


        #endregion

        #endregion
    }
}
