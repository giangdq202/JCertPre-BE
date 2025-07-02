using JCertPreApplication.Application.Features.StudyPlan;
using JCertPreApplication.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [ApiController]
    [Route("api/study-plan")]
    public class StudyPlansController : ControllerBase
    {
        private readonly IStudyPlanService _studyPlanService;

        public StudyPlansController(IStudyPlanService studyPlanService)
        {
            _studyPlanService = studyPlanService ?? throw new ArgumentNullException(nameof(studyPlanService));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStudyPlan([FromBody] StudyPlan studyPlan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // You might want to get the createdByStaffId from the authenticated user
            // studyPlan.createdByStaffId = GetCurrentStaffId();

            var createdStudyPlan = await _studyPlanService.CreateStudyPlanAsync(studyPlan);
            if (createdStudyPlan == null)
            {
                return BadRequest("Failed to create study plan.");
            }
            return CreatedAtAction(nameof(GetStudyPlanById), new { planId = createdStudyPlan.planId }, createdStudyPlan);
        }

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

        [HttpGet("get-all")]
        public async Task<IActionResult> GetAllStudyPlans()
        {
            var studyPlans = await _studyPlanService.GetAllStudyPlansAsync();
            return Ok(studyPlans);
        }

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

        [HttpPut("update/{planId}")]
        public async Task<IActionResult> UpdateStudyPlan(Guid planId, [FromBody] StudyPlan studyPlan)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (planId != studyPlan.planId)
            {
                return BadRequest("Plan ID mismatch.");
            }

            var updatedStudyPlan = await _studyPlanService.UpdateStudyPlanAsync(planId, studyPlan);
            if (updatedStudyPlan == null)
            {
                return NotFound($"Study Plan with ID {planId} not found.");
            }
            return Ok(updatedStudyPlan);
        }

       
    }
}
