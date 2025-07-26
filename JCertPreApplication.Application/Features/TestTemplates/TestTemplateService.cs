using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;

public class TestTemplateService : ITestTemplateService
{
    private readonly ITestTemplateRepository _repo;

    public TestTemplateService(ITestTemplateRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Get all test templates by TestTemplateTypeId.
    /// </summary>
    public async Task<List<TestTemplateDto>> GetAllByTypeIdAsync(Guid testTemplateTypeId)
    {
        try
        {
            var templates = await _repo.GetAllAsync(t => t.TestTemplateTypeId == testTemplateTypeId);
            return templates.Select(MapToDto).ToList();
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
                TestTemplateTypeId = dto.TestTemplateTypeId,
                templateName = dto.templateName,
                durationMinutes = dto.durationMinutes,
                totalScore = dto.totalScore,
                toPassPercentage = dto.toPassPercentage
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
    /// Update a test template by templateId. Only updates provided fields.
    /// </summary>
    public async Task<TestTemplateDto> UpdateAsync(Guid templateId, UpdateTestTemplateDto dto)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(templateId);
            if (entity == null)
                throw ApiException.NotFound("TestTemplate", templateId);

            if (dto.templateName != null)
                entity.templateName = dto.templateName;
            if (dto.durationMinutes.HasValue)
                entity.durationMinutes = dto.durationMinutes.Value;
            if (dto.totalScore.HasValue)
                entity.totalScore = dto.totalScore.Value;
            if (dto.toPassPercentage.HasValue)
                entity.toPassPercentage = dto.toPassPercentage.Value;

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
    /// Delete a test template by templateId.
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

    private static TestTemplateDto MapToDto(TestTemplate t) => new()
    {
        templateId = t.templateId,
        TestTemplateTypeId = t.TestTemplateTypeId,
        templateName = t.templateName,
        durationMinutes = t.durationMinutes,
        totalScore = t.totalScore,
        toPassPercentage = t.toPassPercentage
    };
}