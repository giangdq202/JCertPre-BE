using JCertPreApplication.Application.Dtos.Test;
using JCertPreApplication.Application.Features.Tests;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace JCertPreApplication.API.Controllers
{
    [Route("api/tests")]
    [ApiController]
    [Produces("application/json")]
    public class TestsController : ControllerBase
    {
        private readonly ITestService _testService;

        public TestsController(ITestService testService)
        {
            _testService = testService;
        }

        /// <summary>
        /// Get all tests by user id with paging and search by title.
        /// </summary>
        [HttpGet("by-user/{userId}")]
        [ProducesResponseType(typeof(Pagination<TestDto>), StatusCodes.Status200OK)]
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
        /// Get a test by lesson id.
        /// </summary>
        [HttpGet("by-lesson/{lessonId}")]
        [ProducesResponseType(typeof(TestDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetByLessonId(Guid lessonId)
        {
            var entity = await _testService.GetByLessonIdAsync(lessonId);
            var dto = MapToTestDto(entity!);
            return Ok(dto);
        }

        /// <summary>
        /// Create a test by lesson id. The current authenticated user will be set as CreatedByUser.
        /// </summary>
        [HttpPost("by-lesson/{lessonId}")]
        [ProducesResponseType(typeof(TestDto), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateByLessonId(Guid lessonId, [FromBody] CreateTestDto createTestDto)
        {
            // Get current user id from claims
            var userId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var entity = await _testService.CreateByLessonIdAsync(lessonId, createTestDto, userId);
            var dto = MapToTestDto(entity);
            return CreatedAtAction(nameof(GetByLessonId), new { lessonId = dto.LessonId }, dto);
        }

        /// <summary>
        /// Update a test by test id.
        /// </summary>
        [HttpPut("{testId}")]
        [ProducesResponseType(typeof(TestDto), StatusCodes.Status200OK)]
        public async Task<IActionResult> Update(Guid testId, [FromBody] UpdateTestDto updateTestDto)
        {
            var entity = await _testService.UpdateAsync(testId, updateTestDto);
            var dto = MapToTestDto(entity);
            return Ok(dto);
        }

        /// <summary>
        /// Delete a test by test id.
        /// </summary>
        [HttpDelete("{testId}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> Delete(Guid testId)
        {
            await _testService.DeleteAsync(testId);
            return NoContent();
        }

        // Mapping logic at controller layer
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
                CreatedByUserId = test.createdByUserId
            };
        }
    }
}