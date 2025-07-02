using JCertPreApplication.Application.Features.StudyPlanItem;
using JCertPreApplication.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [ApiController]
    [Route("api/study-plan-item")]
    public class StudyPlanItemsController : ControllerBase
    {
        private readonly IStudyPlanItemService _studyPlanItemService;

        public StudyPlanItemsController(IStudyPlanItemService studyPlanItemService)
        {
            _studyPlanItemService = studyPlanItemService ?? throw new ArgumentNullException(nameof(studyPlanItemService));
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateStudyPlanItem([FromBody] StudyPlanItem studyPlanItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var createdStudyPlanItem = await _studyPlanItemService.CreateStudyPlanItemAsync(studyPlanItem);
                return CreatedAtAction(nameof(GetStudyPlanItemById), new { itemId = createdStudyPlanItem.itemId }, createdStudyPlanItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("get-by-id/{itemId}")]
        public async Task<IActionResult> GetStudyPlanItemById(Guid itemId)
        {
            var studyPlanItem = await _studyPlanItemService.GetStudyPlanItemByIdAsync(itemId);
            if (studyPlanItem == null)
            {
                return NotFound();
            }
            return Ok(studyPlanItem);
        }

        [HttpGet("get-by-plan/{planId}")]
        public async Task<IActionResult> GetStudyPlanItemsByPlanId(Guid planId)
        {
            try
            {
                var studyPlanItems = await _studyPlanItemService.GetStudyPlanItemsByPlanIdAsync(planId);
                if (studyPlanItems == null) // This indicates the planId itself wasn't found by the service
                {
                    return NotFound($"Study Plan with ID {planId} not found.");
                }
                return Ok(studyPlanItems);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPut("update/{itemId}")]
        public async Task<IActionResult> UpdateStudyPlanItem(Guid itemId, [FromBody] StudyPlanItem studyPlanItem)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (itemId != studyPlanItem.itemId)
            {
                return BadRequest("Item ID mismatch.");
            }

            try
            {
                var updatedStudyPlanItem = await _studyPlanItemService.UpdateStudyPlanItemAsync(itemId, studyPlanItem);
                if (updatedStudyPlanItem == null)
                {
                    return NotFound($"Study Plan Item with ID {itemId} not found.");
                }
                return Ok(updatedStudyPlanItem);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpDelete("delete/{itemId}")]
        public async Task<IActionResult> DeleteStudyPlanItem(Guid itemId)
        {
            var result = await _studyPlanItemService.DeleteStudyPlanItemAsync(itemId);
            if (!result)
            {
                return NotFound($"Study Plan Item with ID {itemId} not found.");
            }
            return NoContent();
        }
    }
}
