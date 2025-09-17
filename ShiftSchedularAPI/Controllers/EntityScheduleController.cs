using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;
using ShiftSchedularIL.IServices;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntityScheduleController : ControllerBase
    {
        #region Properties

        private readonly IEntityScheduleService _entityScheduleService;
        private readonly IGeneralService _generalService;

        #endregion

        #region Constructor

        public EntityScheduleController(IEntityScheduleService entityScheduleService, IGeneralService generalService)
        {
            _entityScheduleService = entityScheduleService;
            _generalService = generalService;
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

        #region Assign Entry

        [HttpPost("assign-entry")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AssignEntry(AssignEntryDTO assignEntryDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            BaseResponse<ScheduleEntryDTO> scheduleEntryOp = await _entityScheduleService.AssignEntry(assignEntryDTO);

            if (!scheduleEntryOp.Success)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, scheduleEntryOp.Message);
            }

            return Ok(scheduleEntryOp);
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
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> AddScheduleParticipant(ScheduleParticipantOpDTO scheduleParticipantOp)
        {
            if (scheduleParticipantOp == null)
                return BadRequest("Object is null");

            var scheduleParticipantResult = await _entityScheduleService.AddScheduleParticipant(scheduleParticipantOp);

            if (scheduleParticipantResult.Success)
                return Ok(scheduleParticipantResult);

            else
                return StatusCode(StatusCodes.Status500InternalServerError, scheduleParticipantResult.Message);
        }

        #endregion

        #region Get Schedules

        [HttpPost("get-entity-schedules")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEntitySchedules(ScheduleViewModelRequestDTO requestDTO)
        {
            if (requestDTO == null)
            {
                return BadRequest("Object is null");
            }

            var scheduleEntries = await _entityScheduleService.GetScheduleEntries(requestDTO);

            if (scheduleEntries.Count == 0)
                return NotFound("No schedules were found");

            else
                return Ok(scheduleEntries);
        }

        #endregion

        #region Apply Rotation Cycle

        [HttpPost("apply-rotation-cycle")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ApplyRotationCycle(ApplyRotationCycleDTO rotationCycleDTO)
        {
            if (rotationCycleDTO == null)
            {
                return BadRequest("Object is null");
            }

            var response = await _entityScheduleService.ApplyRotationCycle(rotationCycleDTO);

            if (response.Success)
                return Ok(response);
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        }

        #endregion

        #region Delete Interval Worker Entries

        [HttpDelete("delete-worker-schedule-entries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteWorkerScheduleEntries([FromBody] DeleteIntervalWorkerScheduleEntriesDTO intervalWorkerScheduleEntriesDTO)
        {
            if (intervalWorkerScheduleEntriesDTO == null)
            {
                return BadRequest();
            }

            var response = await _entityScheduleService.DeleteWorkerScheduleEntries(intervalWorkerScheduleEntriesDTO);

            if (response.Success)
                return Ok(response);

            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        }

        #endregion

        #region Delete Schedule

        [HttpDelete("delete-schedule-entries")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteScheduleEntries([FromBody] DeleteIntervalWorkerScheduleEntriesDTO intervalWorkerScheduleEntriesDTO)
        {
            if (intervalWorkerScheduleEntriesDTO == null)
            {
                return BadRequest();
            }

            var response = await _entityScheduleService.DeleteScheduleEntries(intervalWorkerScheduleEntriesDTO);

            if (response.Success)
                return Ok(response);

            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        }

        #endregion

        #region Get Schedule Entry By Id NOT USED 

        [HttpGet("get-schedule-entry-by-id")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetScheduleEntryById(string scheduleEntryId, string languageCode)
        {
            if (string.IsNullOrEmpty(scheduleEntryId))
            {
                return BadRequest();
            }

            var scheduleEntry = await _entityScheduleService.GetScheduleEntryById(_generalService.ParseStringToGuid(scheduleEntryId), languageCode);
            return Ok(scheduleEntry);
        }

        #endregion

        #endregion
    }
}
