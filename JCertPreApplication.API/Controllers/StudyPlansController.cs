using JCertPreApplication.API.Common;
using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Application.Features.StudyPlan;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [ApiController]
    [Route("api/study-plan")]
    [Tags("Studyplans")]
    [Produces("application/json")]
    public class StudyPlansController : ControllerBase
    {
        private readonly IStudyPlanService _studyPlanService;

        public StudyPlansController(IStudyPlanService studyPlanService)
        {
            _studyPlanService = studyPlanService ?? throw new ArgumentNullException(nameof(studyPlanService));
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(StudyPlanDto), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreateStudyPlan([FromBody] StudyPlanDto createDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdStudyPlan = await _studyPlanService.CreateStudyPlanAsync(createDto);
            if (createdStudyPlan == null)
            {
                return BadRequest("Failed to create study plan.");
            }
            return CreatedAtAction(nameof(GetStudyPlanById), new { planId = createdStudyPlan.PlanId }, createdStudyPlan);
        }

        [HttpGet("{planId}")]
        [ProducesResponseType(typeof(StudyPlanDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudyPlanById(Guid planId)
        {
            var studyPlan = await _studyPlanService.GetStudyPlanByIdAsync(planId);
            if (studyPlan == null)
            {
                return NotFound();
            }
            return Ok(studyPlan);
        }

        [HttpGet("get-all")]
        [ProducesResponseType(typeof(IEnumerable<StudyPlanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetAllStudyPlans()
        {
            var studyPlans = await _studyPlanService.GetAllStudyPlansAsync();
            return Ok(studyPlans);
        }

        [HttpGet("get-by-studentid/{studentId}")]
        [ProducesResponseType(typeof(IEnumerable<StudyPlanDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetStudyPlansByStudentId(Guid studentId)
        {
            var studyPlans = await _studyPlanService.GetStudyPlansByStudentIdAsync(studentId);
            if (studyPlans == null || !studyPlans.Any())
            {
                return NotFound("No study plans found for this student.");
            }
            return Ok(studyPlans);
        }

        [HttpPut("update/{planId}")]
        [ProducesResponseType(typeof(StudyPlanDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStudyPlan(Guid planId, [FromBody] UpdateStudyPlanDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedStudyPlan = await _studyPlanService.UpdateStudyPlanAsync(planId, updateDto);
            if (updatedStudyPlan == null)
            {
                return NotFound($"Study Plan with ID {planId} not found.");
            }
            return Ok(updatedStudyPlan);
        }

       
    }
}
