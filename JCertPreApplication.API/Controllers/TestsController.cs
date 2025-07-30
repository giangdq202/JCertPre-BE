using JCertPreApplication.Application.Dtos.Test;
using JCertPreApplication.Application.Features.Tests;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages tests and assessments.
    /// </summary>
    [Route("api/tests")]
    [ApiController]
    [Tags("Tests")]
    [Produces("application/json")]
    public class TestsController : ControllerBase
    {
        private readonly ITestService _testService;

        public TestsController(ITestService testService)
        {
            _testService = testService;
        }

        /// <summary>
        /// Gets all tests for a user with pagination.
        /// </summary>
        /// <param name="userId">User ID.</param>
        /// <param name="searchTerm">Optional search term for test title.</param>
        /// <param name="pageIndex">Page number (default: 1).</param>
        /// <param name="pageSize">Items per page (default: 10).</param>
        /// <returns>Paginated list of tests.</returns>
        [HttpGet("by-user/{userId}")]
        public async Task<IActionResult> GetAllByUserId(
            Guid userId,
            [FromQuery] string? searchTerm,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var pagedEntities = await _testService.GetAllByUserIdAsync(userId, searchTerm, pageIndex, pageSize);

            var pagedDtos = new Pagination<TestDto>
            {
                TotalItemsCount = pagedEntities.TotalItemsCount,
                PageSize = pagedEntities.PageSize,
                PageIndex = pagedEntities.PageIndex,
                Items = pagedEntities.Items.Select(MapToTestDto).ToList()
            };

            return Ok(pagedDtos);
        }

        /// <summary>
        /// Gets a test by lesson ID.
        /// </summary>
        /// <param name="lessonId">Lesson ID.</param>
        /// <returns>Test details.</returns>
        [HttpGet("by-lesson/{lessonId}")]
        public async Task<IActionResult> GetByLessonId(Guid lessonId)
        {
            var entity = await _testService.GetByLessonIdAsync(lessonId);
            var dto = MapToTestDto(entity!);
            return Ok(dto);
        }

        /// <summary>
        /// Creates a test for a lesson.
        /// </summary>
        /// <param name="lessonId">Lesson ID.</param>
        /// <param name="createTestDto">Test details.</param>
        /// <returns>Created test.</returns>
        [HttpPost("by-lesson/{lessonId}")]
        public async Task<IActionResult> CreateByLessonId(Guid lessonId, [FromBody] CreateTestDto createTestDto)
        {
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var entity = await _testService.CreateByLessonIdAsync(lessonId, createTestDto, userId);
            var dto = MapToTestDto(entity);
            return CreatedAtAction(nameof(GetByLessonId), new { lessonId = dto.LessonId }, dto);
        }

        /// <summary>
        /// Updates a test.
        /// </summary>
        /// <param name="testId">Test ID.</param>
        /// <param name="updateTestDto">Updated test details.</param>
        /// <returns>Updated test.</returns>
        [HttpPut("{testId}")]
        public async Task<IActionResult> Update(Guid testId, [FromBody] UpdateTestDto updateTestDto)
        {
            var entity = await _testService.UpdateAsync(testId, updateTestDto);
            var dto = MapToTestDto(entity);
            return Ok(dto);
        }

        /// <summary>
        /// Deletes a test.
        /// </summary>
        /// <param name="testId">Test ID.</param>
        /// <returns>No content on success.</returns>
        [HttpDelete("{testId}")]
        public async Task<IActionResult> Delete(Guid testId)
        {
            await _testService.DeleteAsync(testId);
            return NoContent();
        }

        private static TestDto MapToTestDto(Test test)
        {
            return new TestDto
            {
                TestId = test.testId,
                Title = test.title,
                Description = test.description,
                TestType = test.testType,
                DurationMinutes = test.durationMinutes,
                LessonId = test.lessonId,
                CreatedByUserId = test.createdByUserId,
                AvailableFrom = test.availableFrom,
                AvailableTo = test.availableTo,
                MaxAttempts = test.maxAttempts,
                Status = test.status // <-- Added
            };
        }
    }
}