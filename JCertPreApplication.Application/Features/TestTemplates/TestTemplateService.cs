using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using System.Linq.Expressions;

/// <summary>
/// Service for handling business logic related to TestTemplate entities.
/// Implements exception handling and follows Clean Architecture best practices.
/// </summary>
public class TestTemplateService : ITestTemplateService
{
    private readonly ITestTemplateRepository _repo;

    public TestTemplateService(ITestTemplateRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Get all test templates with search, filter, and paging.
    /// </summary>
    public async Task<Pagination<TestTemplateDto>> GetAllAsync(string? search, CourseLevel? level, TestType? type, int pageIndex, int pageSize)
    {
        try
        {
            // Build predicate for filtering
            Expression<Func<TestTemplate, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(search) || level.HasValue || type.HasValue)
            {
                predicate = t =>
                    (string.IsNullOrEmpty(search) || t.templateName.ToLower().Contains(search.ToLower()))
                    && (!level.HasValue || t.courseLevel == level.Value)
                    && (!type.HasValue || t.testType == type.Value);
            }

            // Fetch paginated templates
            var paginatedTemplates = await _repo.GetPaginationAsync(
                predicate,
                null,
                pageIndex,
                pageSize);

            return new Pagination<TestTemplateDto>
            {
                PageIndex = paginatedTemplates.PageIndex,
                PageSize = paginatedTemplates.PageSize,
                TotalItemsCount = paginatedTemplates.TotalItemsCount,
                Items = paginatedTemplates.Items.Select(MapToDto).ToList()
            };
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("GET_TEST_TEMPLATE_ERROR", ex.Message);
        }
    }

    /// <summary>
    /// Get a test template by id.
    /// </summary>
    public async Task<TestTemplateDto?> GetByIdAsync(Guid templateId)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(templateId);
            return entity == null ? null : MapToDto(entity);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("GET_TEST_TEMPLATE_ERROR", ex.Message);
        }
    }

    /// <summary>
    /// Create a new test template.
    /// </summary>
    public async Task<TestTemplateDto> CreateAsync(CreateTestTemplateDto dto)
    {
        try
        {
            var entity = new TestTemplate
            {
                templateId = Guid.NewGuid(),
                userId = dto.UserId,
                templateName = dto.TemplateName,
                courseLevel = dto.CourseLevel,
                testType = dto.TestType,
                durationMinutes = dto.DurationMinutes,
                description = dto.Description,
                threeFirstParts = dto.ThreeFirstParts,
                fourFirstParts = dto.FourFirstParts,
                reading = dto.Reading,
                listening = dto.Listening,
                total = dto.Total,
                isActive = dto.IsActive,
                createdAt = DateTime.UtcNow
            };
            await _repo.InsertAsync(entity);
            await _repo.SaveChangesAsync();
            return MapToDto(entity);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("CREATE_TEST_TEMPLATE_ERROR", ex.Message);
        }
    }

    /// <summary>
    /// Update an existing test template.
    /// </summary>
    public async Task<TestTemplateDto> UpdateAsync(Guid templateId, UpdateTestTemplateDto dto)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(templateId);
            if (entity == null)
                throw ApiException.NotFound("TestTemplate", templateId);

            entity.templateName = dto.TemplateName;
            entity.courseLevel = dto.CourseLevel;
            entity.testType = dto.TestType;
            entity.durationMinutes = dto.DurationMinutes;
            entity.description = dto.Description;
            entity.threeFirstParts = dto.ThreeFirstParts;
            entity.fourFirstParts = dto.FourFirstParts;
            entity.reading = dto.Reading;
            entity.listening = dto.Listening;
            entity.total = dto.Total;
            entity.isActive = dto.IsActive;

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
            return MapToDto(entity);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("UPDATE_TEST_TEMPLATE_ERROR", ex.Message);
        }
    }

    /// <summary>
    /// Update only the isActive field of a test template by templateId.
    /// </summary>
    public async Task<TestTemplateDto> UpdateIsActiveAsync(Guid templateId, bool isActive)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(templateId);
            if (entity == null)
                throw ApiException.NotFound("TestTemplate", templateId);

            entity.isActive = isActive;

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
            return MapToDto(entity);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("UPDATE_TEST_TEMPLATE_ISACTIVE_ERROR", ex.Message);
        }
    }

    /// <summary>
    /// Delete a test template.
    /// </summary>
    public async Task DeleteAsync(Guid templateId)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(templateId);
            if (entity == null)
                throw ApiException.NotFound("TestTemplate", templateId);
            await _repo.DeleteAsync(entity);
            await _repo.SaveChangesAsync();
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("DELETE_TEST_TEMPLATE_ERROR", ex.Message);
        }
    }

    /// <summary>
    /// Map TestTemplate entity to DTO.
    /// </summary>
    private static TestTemplateDto MapToDto(TestTemplate t) => new()
    {
        TemplateId = t.templateId,
        UserId = t.userId,
        TemplateName = t.templateName,
        CourseLevel = t.courseLevel,
        TestType = t.testType,
        DurationMinutes = t.durationMinutes,
        Description = t.description,
        ThreeFirstParts = t.threeFirstParts,
        FourFirstParts = t.fourFirstParts,
        Reading = t.reading,
        Listening = t.listening,
        Total = t.total,
        IsActive = t.isActive,
        CreatedAt = t.createdAt
    };
}