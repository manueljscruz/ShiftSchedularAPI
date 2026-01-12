using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using ShiftSchedularBLL.IService;
using ShiftSchedularEntity.Entities;
using ShiftSchedularEntity.Models.API_Management;

namespace ShiftSchedularAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    [EnableRateLimiting("general")]
    public class SkillController : ControllerBase
    {
        private readonly ISkillService _skillService;

        #region Constructor

        public SkillController(ISkillService skillService)
        {
            _skillService = skillService;
        }

        #endregion

        #region Methods

        #region Get Skill By Id

        /// <summary>
        /// Gets the Skill instance by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns></returns>
        [HttpGet("get-by-id/{id}")]
        [ProducesResponseType(200, Type = typeof(Skill))]
        public async Task<IActionResult> GetSkillById(int id)
        {
            var skill = await _skillService.GetSkillById(id);

            if (skill == null)
            {
                return NotFound();
            }

            return Ok(skill);
        }

        #endregion

        #region Get All Skills

        /// <summary>
        /// Get all skills records
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllSkills()
        {
            var skills = await _skillService.GetAllSkills();

            return Ok(skills);
        }

        #endregion

        #region Get Skills By Localization

        /// <summary>
        /// Get all skills records by localization code
        /// </summary>
        /// <returns></returns>
        [HttpGet("get-all-skills-by-localization/{lcode}")]
        [ProducesResponseType(200)]
        public async Task<IActionResult> GetAllSkillsByLocalization(string lcode)
        {
            var skillsLocalization = await _skillService.GetAllSkillsByLocalization(lcode);

            return Ok(skillsLocalization);
        }

        #endregion

        #region Add Skill

        /// <summary>
        /// Adds a new Skill to the database
        /// </summary>
        /// <param name="strNewSkillValue"></param>
        /// <returns></returns>
        [HttpPost("add")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddSkill(SkillSubmissionModel newSkill)
        {
            int newSkillId = await _skillService.AddSkill(newSkill);

            return Created($"/api/skill/{newSkillId}", "Skill Added");
        }

        #endregion

        #region Add Skill Localization

        /// <summary>
        /// Adds a new skill localization entry
        /// </summary>
        /// <param name="skillLocalizationSubmission"></param>
        /// <returns></returns>
        [HttpPost("add-skill-localization")]
        [ProducesResponseType(201)]
        public async Task<IActionResult> AddSkillLocalization(SkillLocalizationSubmissionModel skillLocalizationSubmission)
        {
            bool result = await _skillService.AddSkillLocalization(skillLocalizationSubmission);

            if (result)
                return Created($"/api/skill/add-skill-localization", "Skill Localization entry submitted");
            else
                return BadRequest("Error occured when creating a skill localization entry");
        }

        #endregion

        #region Update Skill

        /// <summary>
        /// Updates the skill instance
        /// </summary>
        /// <param name="skillInstance"></param>
        /// <returns></returns>
        [HttpPut("update")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> UpdateGender(Skill skillInstance)
        {
            await _skillService.UpdateSkill(skillInstance);

            return NoContent();
        }

        #endregion

        #region Delete Skill by Id

        /// <summary>
        /// Delete instance by identifier
        /// </summary>
        /// <param name="id">Identifier</param>
        /// <returns></returns>
        [HttpDelete("delete-by-id/{id}")]
        [ProducesResponseType(204)]
        public async Task<IActionResult> DeleteSkillById(int id)
        {
            await _skillService.DeleteSkill(id);

            return NoContent();
        }

        #endregion

        #endregion

    }
}
