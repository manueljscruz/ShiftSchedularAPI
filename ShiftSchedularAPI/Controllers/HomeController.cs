using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Models.ViewModels;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IHomeService _homeService;

        public HomeController(IHomeService homeService)
        {
            _homeService = homeService;
        }

        /// <summary>
        /// Returns a home view model class for the home page
        /// </summary>
        /// <param name="lcode">Language code</param>
        /// <returns>Genders and subscription plans in localized format</returns>
        [HttpGet("get-home-view-model/{lcode}")]
        [EnableRateLimiting("public")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetHomeViewModel(string lcode)
        {
            if (string.IsNullOrEmpty(lcode))
                return BadRequest("Language code required.");

            HomeViewModel homeViewModel = await _homeService.GetHomeViewModel(lcode);
            if (homeViewModel == null)
                return NotFound("No data was found to be returned");

            return Ok(homeViewModel);
        }

        [HttpPost("send-email-test")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> SendEmailTest(string email)
        {
            await _homeService.SendEmailTest(email);

            return Ok();
        }
    }
}
