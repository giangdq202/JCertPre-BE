using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Application.Features.StudyPlanItem;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [ApiController]
    [Route("api/study-plan-item")]
    [Tags("StudyplanItems")]
    [Produces("application/json")]
    public class StudyPlanItemsController : ControllerBase
    {
        private readonly IStudyPlanItemService _studyPlanItemService;

        public StudyPlanItemsController(IStudyPlanItemService studyPlanItemService)
        {
            _studyPlanItemService = studyPlanItemService ?? throw new ArgumentNullException(nameof(studyPlanItemService));
        }

        [HttpPost("create")]
        [ProducesResponseType(typeof(StudyPlanItemDto), StatusCodes.Status201Created)]
        
        public async Task<IActionResult> CreateStudyPlanItem(Guid planId, int sequence, string itemType, Guid? courseId, Guid? testId, ItemStatus status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdStudyPlanItem = await _studyPlanItemService.CreateStudyPlanItemAsync(planId, sequence, itemType, courseId, testId, status);
            return CreatedAtAction(nameof(GetStudyPlanItemById), new { itemId = createdStudyPlanItem.ItemId }, createdStudyPlanItem);
        }

        [HttpGet("get-by-id/{itemId}")]
        [ProducesResponseType(typeof(StudyPlanItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

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
        [ProducesResponseType(typeof(IEnumerable<StudyPlanItemDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> GetStudyPlanItemsByPlanId(Guid planId)
        {
            var studyPlanItems = await _studyPlanItemService.GetStudyPlanItemsByPlanIdAsync(planId);
            return Ok(studyPlanItems);
        }

        [HttpPut("update/{itemId}")]
        [ProducesResponseType(typeof(StudyPlanItemDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]

        public async Task<IActionResult> UpdateStudyPlanItem(Guid itemId, [FromBody] UpdateStudyPlanItemDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedStudyPlanItem = await _studyPlanItemService.UpdateStudyPlanItemAsync(itemId, updateDto);
            return Ok(updatedStudyPlanItem);
        }

        [HttpDelete("delete/{itemId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
