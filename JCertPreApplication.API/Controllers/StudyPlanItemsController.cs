using JCertPreApplication.Application.Dtos.StudyPlan;
using JCertPreApplication.Application.Features.StudyPlanItem;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages study plan items within a study plan.
    /// </summary>
    [ApiController]
    [Route("api/study-plan-items")]
    [Tags("StudyplanItems")]
    [Produces("application/json")]
    public class StudyPlanItemsController : ControllerBase
    {
        private readonly IStudyPlanItemService _studyPlanItemService;

        public StudyPlanItemsController(IStudyPlanItemService studyPlanItemService)
        {
            _studyPlanItemService = studyPlanItemService ?? throw new ArgumentNullException(nameof(studyPlanItemService));
        }

        /// <summary>
        /// Creates a new study plan item.
        /// </summary>
        /// <param name="planId">Study plan ID.</param>
        /// <param name="sequence">Item sequence number.</param>
        /// <param name="itemType">Type of study plan item.</param>
        /// <param name="courseId">Optional course ID.</param>
        /// <param name="testId">Optional test ID.</param>
        /// <param name="status">Item status.</param>
        /// <returns>Created study plan item.</returns>
        [HttpPost("create")]
        public async Task<IActionResult> CreateStudyPlanItem(Guid planId, int sequence, string itemType, Guid? courseId, Guid? testId, ItemStatus status)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdStudyPlanItem = await _studyPlanItemService.CreateStudyPlanItemAsync(planId, sequence, itemType, courseId, testId, status);
            return CreatedAtAction(nameof(GetStudyPlanItemById), new { itemId = createdStudyPlanItem.ItemId }, createdStudyPlanItem);
        }

        /// <summary>
        /// Gets a study plan item by ID.
        /// </summary>
        /// <param name="itemId">Study plan item ID.</param>
        /// <returns>Study plan item details.</returns>
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

        /// <summary>
        /// Gets all items in a study plan.
        /// </summary>
        /// <param name="planId">Study plan ID.</param>
        /// <returns>List of study plan items.</returns>
        [HttpGet("get-by-plan/{planId}")]
        public async Task<IActionResult> GetStudyPlanItemsByPlanId(Guid planId)
        {
            var studyPlanItems = await _studyPlanItemService.GetStudyPlanItemsByPlanIdAsync(planId);
            return Ok(studyPlanItems);
        }

        /// <summary>
        /// Updates a study plan item.
        /// </summary>
        /// <param name="itemId">Study plan item ID.</param>
        /// <param name="updateDto">Updated item details.</param>
        /// <returns>Updated study plan item.</returns>
        [HttpPut("update/{itemId}")]
        public async Task<IActionResult> UpdateStudyPlanItem(Guid itemId, [FromBody] UpdateStudyPlanItemDto updateDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedStudyPlanItem = await _studyPlanItemService.UpdateStudyPlanItemAsync(itemId, updateDto);
            return Ok(updatedStudyPlanItem);
        }

        /// <summary>
        /// Deletes a study plan item.
        /// </summary>
        /// <param name="itemId">Study plan item ID.</param>
        /// <returns>No content on success.</returns>
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
