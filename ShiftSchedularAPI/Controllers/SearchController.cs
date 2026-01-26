using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        #region Search

        /// <summary>
        /// Main Search for entities and workers
        /// </summary>
        /// <param name="searchRequest"></param>
        /// <returns></returns>
        [HttpPost("search")]
        [Authorize]
        [EnableRateLimiting("general")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Search([FromBody] SearchRequestDTO searchRequest)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            BaseResponse<PagedList<SearchResultDTO>> response = await _searchService.Search(searchRequest);

            if (response.Success)
                return Ok(response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        }

        #endregion

        #region Search Worker 

        /// <summary>
        /// Retrieves the public profile of a worker by worker ID and language code.
        /// </summary>
        /// <param name="workerId">The unique identifier of the worker.</param>
        /// <param name="lanaguageCode">The language code for the profile information.</param>
        /// <returns>An IActionResult containing the worker's public profile if found; otherwise, a BadRequest or NotFound
        /// result.</returns>
        [HttpGet("worker/{workerId}/{languageCode}")]
        [Authorize]
        [EnableRateLimiting("general")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPublicWorkerProfile([FromQuery] string workerId, [FromQuery] string lanaguageCode)
        {
            if (string.IsNullOrEmpty(workerId))
            {
                return BadRequest();
            }

            BaseResponse<WorkerPublicProfileDTO> workerProfileResponse = await _searchService.GetPublicProfileWorker(workerId, lanaguageCode);

            if (workerProfileResponse.Success)
            {
                return Ok(workerProfileResponse);
            }
            else
                return NotFound(workerProfileResponse.Message);

        }

        #endregion

        #region Get Entity Profile

        /// <summary>
        /// Retrieves the public profile of an entity based on the provided request data.
        /// </summary>
        /// <param name="viewModelRequest">The request containing information needed to identify the entity.</param>
        /// <returns>An IActionResult containing the entity's public profile if found, a bad request if the input is invalid, or
        /// not found if the entity does not exist.</returns>
        [HttpPost("entity")]
        [Authorize]
        [EnableRateLimiting("general")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetPublicEntityProfile([FromBody] BaseViewModelRequest viewModelRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            BaseResponse<EntityPublicProfileDTO> entityProfileResponse = await _searchService.GetPublicEntityProfile(viewModelRequest);

            if (entityProfileResponse.Success)
            {
                return Ok(entityProfileResponse);
            }
            else
                return NotFound(entityProfileResponse.Message);
        }

        #endregion
    }
}
