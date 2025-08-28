using JCertPreApplication.Application.Dtos.TestTemplateType;
using JCertPreApplication.Application.Dtos.TestTemplateTypes;
using JCertPreApplication.Application.Features.TestTemplateTypes;
using JCertPreApplication.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Manages test template type operations.
    /// </summary>
    [Route("api/test-template-types")]
    [ApiController]
    [Tags("TestTemplateTypes")]
    [Produces("application/json")]
    [Authorize]
    public class TestTemplateTypeController : ControllerBase
    {
        private readonly ITestTemplateTypeService _service;

        public TestTemplateTypeController(ITestTemplateTypeService service)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
        }

        /// <summary>
        /// Get all test template types with search, filter, and paging.
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] CourseLevel? level, [FromQuery] TestType? type, [FromQuery] bool? isActive, [FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllAsync(search, level, type, isActive, pageIndex, pageSize);
            return Ok(result);
        }

        /// <summary>
        /// Create a new test template type.
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> Create([FromBody] CreateTestTemplateTypeDto dto)
        {
            // Get the authenticated user's ID from claims
            var claimUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (claimUserId == null || !Guid.TryParse(claimUserId, out var authenticatedUserId) || authenticatedUserId != dto.userId)
            {
                return Forbid();
            }
            var result = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetAll), new { }, result);
        }

        /// <summary>
        /// Update a test template type by id.
        /// </summary>
        [HttpPut("{testTemplateTypeId:guid}")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> Update(Guid testTemplateTypeId, [FromBody] UpdateTestTemplateTypeDto dto)
        {
            var result = await _service.UpdateAsync(testTemplateTypeId, dto);
            return Ok(result);
        }

        /// <summary>
        /// Delete a test template type by id.
        /// </summary>
        [HttpDelete("{testTemplateTypeId:guid}")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> Delete(Guid testTemplateTypeId)
        {
            await _service.DeleteAsync(testTemplateTypeId);
            return NoContent();
        }

        /// <summary>
        /// Update only the isActive field of a test template type by id.
        /// </summary>
        [HttpPatch("{testTemplateTypeId:guid}/is-active")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> UpdateIsActive(Guid testTemplateTypeId, [FromQuery] bool isActive)
        {
            var result = await _service.UpdateIsActiveAsync(testTemplateTypeId, isActive);
            return Ok(result);
        }

        /// <summary>
        /// Gets summary info for a test template type and its templates.
        /// </summary>
        [HttpGet("summary")]
        [Authorize(Roles = "STUDENT,INSTRUCTOR,ACADEMIC_MANAGER")]
        public async Task<IActionResult> GetTemplateTypeSummary([FromQuery] CourseLevel courseLevel, [FromQuery] TestType testType)
        {
            var result = await _service.GetTemplateTypeSummaryAsync(courseLevel, testType);
            if (result == null)
                return NotFound();
            return Ok(result);
        }

        /// <summary>
        /// Verify a test template type by id.
        /// </summary>
        [HttpPost("{testTemplateTypeId:guid}/verify")]
        [Authorize(Roles = "ACADEMIC_MANAGER")]
        public async Task<IActionResult> Verify(Guid testTemplateTypeId, [FromQuery] Guid userId)
        {
            // Get the authenticated user's ID from claims
            var claimUserId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (claimUserId == null || !Guid.TryParse(claimUserId, out var authenticatedUserId) || authenticatedUserId != userId)
            {
                return Forbid();
            }
            var result = await _service.VerifyAsync(testTemplateTypeId, userId);
            return Ok(result);
        }
    }
}
