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

        [HttpPost("get-entity-schedule-view-model")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetEntityScheduleViewModel(BaseViewModelRequest viewModelRequest)
        {
            var viewModel = await _entityScheduleService.GetEntityScheduleViewModel(viewModelRequest);

            return Ok(viewModel);
        }

        #endregion
    }
}
