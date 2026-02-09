using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models;
using ShiftSchedularEntity.Models.DataTransferObjects.Incoming;
using ShiftSchedularEntity.Models.DataTransferObjects.Outgoing;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableRateLimiting("general")]
    public class HolidayController : ControllerBase
    {
        private readonly IHolidayService _holidayService;

        public HolidayController(IHolidayService holidayService)
        {
            _holidayService = holidayService;
        }

        #region Methods

        #region Get Entity Holidays View Model

        [Authorize]
        [HttpPost("get-entity-holidays-view-model")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntityHolidaysViewModel([FromBody] BaseViewModelRequest viewModelRequestDTO)
        {
            if (viewModelRequestDTO == null)
            {
                return BadRequest();
            }

            var holidaysViewModel = await _holidayService.GetEntityHolidaysViewModel(viewModelRequestDTO);

            if (holidaysViewModel == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "");
            }
            else
                return Ok(holidaysViewModel);
        }

        #endregion

        #region Add Holiday Type

        [HttpPost("holiday-type/add/{strNewHolidayType}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddHolidayType([FromRoute] string strNewHolidayType)
        {
            if (string.IsNullOrEmpty(strNewHolidayType))
                return BadRequest();

            int newHolidayTypeId = await _holidayService.AddHolidayType(strNewHolidayType);

            return Created($"/api/holiday/holiday-type/{newHolidayTypeId}", "Holiday Type Added");
        }

        #endregion

        #region Get Holiday Type By Id

        [HttpGet("holiday-type/get-by-id/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHolidayTypeById([FromRoute] int id)
        {
            if (id == 0)
                return BadRequest();

            HolidayType holidayType = await _holidayService.GetHolidayTypeById(id);

            if (holidayType == null)
                return NotFound();

            return Ok(holidayType);
        }

        #endregion

        #region Update Holiday Type

        [HttpPut("holiday-type/update")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHolidayType([FromBody] HolidayType holidayType)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            BaseResponse<bool> result = await _holidayService.UpdateHolidayType(holidayType);

            if (result.Success)
                return NoContent();
            else
                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
        }

        #endregion

        #region Delete Holiday Type

        [HttpDelete("holiday-type/delete/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHolidayType([FromRoute] int id)
        {
            if(id <= 0)
            {
                return BadRequest();
            }

            BaseResponse<bool> response = await _holidayService.DeleteHolidayType(id);

            if (response.Success)
                return NoContent();

            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        }

        #endregion

        #region Add Holiday Behaviour

        [HttpPost("holiday-behaviour/add/{strNewHolidayBehaviour}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddHolidayBehaviour([FromRoute] string strNewHolidayBehaviour)
        {
            if (string.IsNullOrEmpty(strNewHolidayBehaviour))
                return BadRequest();

            int newHolidayBehaviourId = await _holidayService.AddHolidayBehaviour(strNewHolidayBehaviour);

            return Created($"/api/holiday/holiday-behaviour/{newHolidayBehaviourId}", "Holiday Behaviour Added");
        }

        #endregion

        #region Get Holiday Behaviour By Id

        [HttpGet("holiday-behaviour/get-by-id/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHolidayBehaviourById([FromRoute] int id)
        {
            if (id == 0)
                return BadRequest();

            HolidayBehaviour holidayBehaviour = await _holidayService.GetHolidayBehaviourById(id);

            if (holidayBehaviour == null)
                return NotFound();

            return Ok(holidayBehaviour);
        }

        #endregion

        #region Update Holiday Behaviour

        [HttpPut("holiday-behaviour/update")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHolidayBehaviour([FromBody] HolidayBehaviour holidayBehaviour)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            BaseResponse<bool> result = await _holidayService.UpdateHolidayBehaviour(holidayBehaviour);

            if (result.Success)
                return NoContent();
            else
                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
        }

        #endregion

        #region Delete Holiday Behaviour

        [HttpDelete("holiday-behaviour/delete/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHolidayBehaviour([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            BaseResponse<bool> response = await _holidayService.DeleteHolidayBehaviour(id);

            if (response.Success)
                return NoContent();

            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        }

        #endregion


        #region Add Holiday Catalog

        [HttpPost("holiday-catalog/add")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddHolidayCatalog([FromBody] AddHolidayCatalogDTO addHolidayCatalogDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            int newHolidayCatalogId = await _holidayService.AddHolidayCatalog(addHolidayCatalogDTO);

            return Created($"/api/holiday/holiday-catalog/{newHolidayCatalogId}", "Holiday Catalog Added");
        }

        #endregion

        #region Get Holiday Catalog By Id

        [HttpGet("holiday-catalog/get-by-id/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHolidayCatalogById([FromRoute] int id)
        {
            if (id == 0)
                return BadRequest();

            HolidayCatalog holidayCatalog = await _holidayService.GetHolidayCatalogById(id);

            if (holidayCatalog == null)
                return NotFound();

            return Ok(holidayCatalog);
        }

        #endregion

        #region Update Holiday Catalog 

        [HttpPut("holiday-catalog/update")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> UpdateHolidayCatalog([FromBody] HolidayCatalog holidayCatalog)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            BaseResponse<bool> result = await _holidayService.UpdateHolidayCatalog(holidayCatalog);

            if (result.Success)
                return NoContent();
            else
                return StatusCode(StatusCodes.Status500InternalServerError, result.Message);
        }

        #endregion

        #region Delete Holiday Catalog

        [HttpDelete("holiday-catalog/delete/{id}")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteHolidayCatalog([FromRoute] int id)
        {
            if (id <= 0)
            {
                return BadRequest();
            }

            BaseResponse<bool> response = await _holidayService.DeleteHolidayCatalog(id);

            if (response.Success)
                return NoContent();

            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        }

        #endregion


        #region Add Entity Holiday

        [HttpPost("entity-holiday/add")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> AddEntityHoliday([FromBody] AddEntityHolidayDTO addEntityHolidayDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            BaseResponse<EntityHolidayDTO> response = await _holidayService.AddEntityHoliday(addEntityHolidayDTO);

            if (response.Success)
                return Ok(response);

            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        }

        #endregion

        #region Get Entity Holiday By Id

        [HttpGet("entity-holiday/get-by-id/{id}")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntityHolidayById([FromRoute] Guid id)
        {
            if (id == Guid.Empty)
                return BadRequest();

            EntityHolidayDTO entityHoliday = await _holidayService.GetEntityHolidayById(id);

            if (entityHoliday == null)
                return NotFound();

            else
                return Ok(entityHoliday);
        }

        #endregion

        #region Get Entity Holidays Pagination

        [HttpPost("entity-holiday/get-by-pagination")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetEntityHolidaysPagination([FromBody] PagedModelRequest entityHolidaysPaginationRequest)
        {
            if(!ModelState.IsValid)
            {
                return BadRequest();
            }

            PagedList<EntityHolidayDTO> entityHolidays = await _holidayService.GetEntityHolidaysPagination(entityHolidaysPaginationRequest);

            return Ok(entityHolidays);
        }

        #endregion

        #region Update Entity Holiday

        [HttpPut("entity-holiday/update")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> UpdateEntityHoliday([FromBody] EntityHolidayDTO entityHolidayDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();


            BaseResponse<bool> response = await _holidayService.UpdateEntityHoliday(entityHolidayDTO);

            if (response.Success)
                return NoContent();
            else
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
        }

        #endregion

        #region Delete Entity Holiday

        [HttpDelete("entity-holiday/delete")]
        [Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteEntityHoliday([FromBody] EntityHolidayDTO entityHolidayDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            BaseResponse<bool> response = await _holidayService.DeleteEntityHoliday(entityHolidayDTO);

            if (response.Success)
                return Ok(response);
            else
            {
                return StatusCode(StatusCodes.Status500InternalServerError, response.Message);
            }
        }

        #endregion

        #endregion

    }
}
