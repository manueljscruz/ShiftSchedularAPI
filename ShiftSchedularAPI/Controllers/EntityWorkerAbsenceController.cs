using Microsoft.AspNetCore.Mvc;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EntityWorkerAbsenceController : ControllerBase
    {
        private readonly IEntityWorkerAbsenceService _entityWorkerAbsenceService;

        #region Constructor

        public EntityWorkerAbsenceController(IEntityWorkerAbsenceService entityWorkerAbsenceService)
        {
            _entityWorkerAbsenceService = entityWorkerAbsenceService;
        }

        #endregion

        #region Methods

        #region Get Entity Worker Absence By Id

        [HttpGet("get-by-id/{id}/{lcode}")]
        [ProducesResponseType(200, Type = typeof(EntityWorkerAbsenceDTO))]
        public async Task<IActionResult> GetEntityWorkerAbsenceById(string id, string lcode)
        {
            var entityWorkerAbsence = await _entityWorkerAbsenceService.GetEntityWorkerAbsenceById(id, lcode);
            return Ok(entityWorkerAbsence);
        }

        #endregion

        #region Get Entity Worker Absences View Model

        [HttpPost("get-entity-worker-absence-model")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetEntityWorkerAbsencesViewModel(BaseViewModelRequest viewModelRequestDTO)
        {
            var viewModel = await _entityWorkerAbsenceService.GetEntityWorkerAbsenceViewModel(viewModelRequestDTO);
            return Ok(viewModel);
        }

        #endregion

        #region Add Entity Worker Absence

        [HttpPost("add-entity-worker-absence")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> AddEntityWorkerAbsence([FromBody] AddEntityWorkerAbsenceDTO addEntityWorkerAbsence)
        {
            var newEntityWorkerAbsence = await _entityWorkerAbsenceService.AddEntityWorkerAbsence(addEntityWorkerAbsence);
            return Ok(newEntityWorkerAbsence);
        }

        #endregion

        #region Update Entity Worker Absence

        [HttpPut("update-entity-worker-absence")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateEntityWorkerAbsence(EntityWorkerAbsenceDTO entityWorkerAbsenceDTO)
        {
            var updatedAbsence = await _entityWorkerAbsenceService.UpdateEntityWorkerAbsence(entityWorkerAbsenceDTO);
            return Ok(updatedAbsence);
        }

        #endregion

        #region Absence Approval Decision

        [HttpPut("absence-approval-decision")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> AbsenceApprovalDecision(AbsenceApprovalDecisionDTO absenceApprovalDecisionDTO)
        {
            var response = await _entityWorkerAbsenceService.AbsenceApprovalDecision(absenceApprovalDecisionDTO);
            return Ok(response);
        }

        #endregion

        #region Delete Entity Worker Absence

        [HttpDelete("delete-entity-worker-absence/{absenceId}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteEntityWorkerAbsence(string absenceId)
        {
            var response = await _entityWorkerAbsenceService.DeleteEntityWorkerAbsence(absenceId);
            return Ok(response);
        }

        #endregion

        #endregion
    }
}
