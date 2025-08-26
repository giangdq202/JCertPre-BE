using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Application.Features.StudyPlan;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages study plans for students.
    /// </summary>
    [ApiController]
    [Route("api/study-plans")]
    [Tags("Studyplans")]
    [Produces("application/json")]
    [Authorize]
    public class StudyPlansController : ControllerBase
    {
        private readonly IStudyPlanService _studyPlanService;

        public StudyPlansController(IStudyPlanService studyPlanService)
        {
            _studyPlanService = studyPlanService ?? throw new ArgumentNullException(nameof(studyPlanService));
        }

        /// <summary>
        /// Creates a new study plan.
        /// </summary>
        /// <param name="createDto">Study plan details.</param>
        /// <returns>Created study plan.</returns>
        [HttpPost("create")]
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

        /// <summary>
        /// Gets a study plan by ID.
        /// </summary>
        /// <param name="planId">Study plan ID.</param>
        /// <returns>Study plan details.</returns>
        [HttpGet("{planId}")]
        public async Task<IActionResult> GetStudyPlanById(Guid planId)
        {
            var studyPlan = await _studyPlanService.GetStudyPlanByIdAsync(planId);
            if (studyPlan == null)
            {
                return NotFound();
            }
            return Ok(studyPlan);
        }

        /// <summary>
        /// Gets all study plans.
        /// </summary>
        /// <returns>List of all study plans.</returns>
        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllStudyPlans()
        {
            var studyPlans = await _studyPlanService.GetAllStudyPlansAsync();
            return Ok(studyPlans);
        }

        /// <summary>
        /// Gets all study plans for a student.
        /// </summary>
        /// <param name="studentId">Student ID.</param>
        /// <returns>List of student's study plans.</returns>
        [HttpGet("get-by-studentid/{studentId}")]
        public async Task<IActionResult> GetStudyPlansByStudentId(Guid studentId)
        {
            var studyPlans = await _studyPlanService.GetStudyPlansByStudentIdAsync(studentId);
            if (studyPlans == null || !studyPlans.Any())
            {
                return NotFound("No study plans found for this student.");
            }
            return Ok(studyPlans);
        }

        /// <summary>
        /// Updates a study plan.
        /// </summary>
        /// <param name="planId">Study plan ID.</param>
        /// <param name="updateDto">Updated study plan details.</param>
        /// <returns>Updated study plan.</returns>
        [HttpPut("update/{planId}")]
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
