using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("general")]
    public class EntityTypeController : ControllerBase
    {
        private readonly IEntityTypeService _entityTypeService;

        #region Constructor

        public EntityTypeController(IEntityTypeService entityTypeService)
        {
            _entityTypeService = entityTypeService;
        }

        #endregion

        #region Methods

        #region Get Entity Type By Id 

        /// <summary>
        /// Gets Entity Type instance by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("get-by-id/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200, Type = typeof(EntityType))]
        public async Task<IActionResult> GetEntityTypeById(int id)
        {
            var entityType = await _entityTypeService.GetEntityTypeById(id);

            if(entityType == null)
            {
                return NotFound();
            }

            return Ok(entityType);
        }

        #endregion

        #region Get All Entity Types

        /// <summary>
        /// Get all entity type records
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllEntityTypes()
        {
            var entityTypes = await _entityTypeService.GetAllEntityTypes();

            return Ok(entityTypes);
        }

        #endregion

        #region Get Entity Types By Localization

        /// <summary>
        /// Get all entity type records by localization code
        /// </summary>
        /// <param name="lcode"></param>
        /// <returns></returns>
        [Authorize(Roles = "User")]
        [HttpGet("get-all-entity-types-by-localization/{lcode}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllEntityTypesByLocalization(string lcode)
        {
            var entityTypesLocalization = await _entityTypeService.GetAllEntityTypesByLocalization(lcode);

            return Ok(entityTypesLocalization);
        }

        #endregion

        #region Add Entity Type

        /// <summary>
        /// Add Entity Type to the database
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddEntityType(string strNewEntityType)
        {
            int newEntityTypeId = await _entityTypeService.AddEntityType(strNewEntityType);

            return Created($"/api/entityType/{newEntityTypeId}", "Entity Type Added");
        }

        #endregion

        #region Update Entity Type

        /// <summary>
        /// Updates the entity type instance
        /// </summary>
        /// <param name="entityType"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateEntityType(EntityType entityType)
        {
            await _entityTypeService.UpdateEntityType(entityType);

            return NoContent();
        }

        #endregion

        #region Delete Entity Type By Id

        /// <summary>
        /// Delete instance by identifier
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("delete-by-id/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteEntityTypeById(int id)
        {
            await _entityTypeService.DeleteEntityType(id);

            return NoContent();
        }

        #endregion

        #endregion
    }
}
