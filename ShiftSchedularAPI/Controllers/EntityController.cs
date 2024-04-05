using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects;

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

        #region Add Entity

        [HttpPost("add")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddEntity(NewEntityDTO newEntity)
        {
            BaseResponse<Entity> response = await _entityService.AddEntity(newEntity);

            return Ok(response);
        }

        #endregion

        #region Update Entity

        [HttpPut("update")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateEntity(Entity entity)
        {
            BaseResponse<bool> response = await _entityService.UpdateEntity(entity);

            if (response.Success)
                return NoContent();
            else
                return Ok(response);
        }

        #endregion

        #region Delete Entity by Id

        [HttpDelete("delete-by-id/{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteEntityById(string entityId)
        {
            BaseResponse<bool> response = await _entityService.DeleteEntityById(entityId);

            if (response.Success)
                return NoContent();
            else
                return Ok(response);

        }

        #endregion

        #endregion
    }
}
