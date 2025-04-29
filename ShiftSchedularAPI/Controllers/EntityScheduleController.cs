using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntityScheduleController : ControllerBase
    {
        #region Properties

        private readonly IEntityScheduleService _entityScheduleService;

        #endregion

        #region Constructor

        public EntityScheduleController(IEntityScheduleService entityScheduleService)
        {
            _entityScheduleService = entityScheduleService;
        }

        #endregion

        #region Methods

        #region Get Entity Schedule View Model

        /// <summary>
        /// Get Entity Schedule View Model
        /// </summary>
        /// <param name="viewModelRequest"></param>
        /// <returns></returns>
        [HttpPost("get-entity-schedule-view-model")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEntityScheduleViewModel(ScheduleViewModelRequestDTO viewModelRequest)
        {
            var viewModel = await _entityScheduleService.GetEntityScheduleViewModel(viewModelRequest);

            return Ok(viewModel);
        }

        #endregion

        #region Get Schedule Entry By Id

        [HttpGet("get-schedule-entry-by-id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetScheduleEntryById(string scheduleEntryId, string languageCode)
        {
            var scheduleEntry = await _entityScheduleService.GetScheduleEntryById(scheduleEntryId, languageCode);
            return Ok(scheduleEntry);
        }

        #endregion

        #region Add Schedule Entry

        [HttpPost("add-schedule-entry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddScheduleEntry(AddScheduleEntryDTO addScheduleEntryDTO)
        {
            var scheduleEntryResult = await _entityScheduleService.AddScheduleEntry(addScheduleEntryDTO);
            return Ok(scheduleEntryResult);
        }

        #endregion

        #region Add Schedule Participant

        [HttpPost("add-schedule-participant")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> AddScheduleParticipant(ScheduleParticipantOpDTO scheduleParticipantOp)
        {
            var scheduleParticipantResult = await _entityScheduleService.AddScheduleParticipant(scheduleParticipantOp);
            return Ok(scheduleParticipantResult);
        }

        #endregion

        #region Generate Entity Schedule

        /// <summary>
        /// Generates an entity schedule based on the provided parameters.
        /// </summary>
        /// <param name="createEntityScheduleDTO">The DTO containing the details for creating the entity schedule.</param>
        /// <returns>A response containing the result of the schedule generation.</returns>
        [HttpPost("generate-entity-schedule")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GenerateEntitySchedule(CreateEntityScheduleDTO createEntityScheduleDTO)
        {
            var scheduleEntryResult = await _entityScheduleService.CreateEntitySchedule(createEntityScheduleDTO);

            return Ok(scheduleEntryResult);
        }

        #endregion

        #endregion
    }
}
