using JCertPreApplication.Application.Dtos.TestTemplateType;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Application.Dtos.TestTemplateTypes;
namespace JCertPreApplication.Application.Features.TestTemplateTypes;
public interface ITestTemplateTypeService
{
    Task<Pagination<TestTemplateTypeDto>> GetAllAsync(string? search, CourseLevel? level, TestType? type, bool? isActive, int pageIndex, int pageSize);
    Task<TestTemplateTypeDto> CreateAsync(CreateTestTemplateTypeDto dto);
    Task<TestTemplateTypeDto> UpdateAsync(Guid testTemplateTypeId, UpdateTestTemplateTypeDto dto);
    Task DeleteAsync(Guid testTemplateTypeId);
    Task<TestTemplateTypeDto> UpdateIsActiveAsync(Guid testTemplateTypeId, bool isActive);
    Task<TestTemplateTypeDto> VerifyAsync(Guid testTemplateTypeId, Guid userId);

    /// <summary>
    /// Gets summary info for a test template type and its templates.
    /// </summary>
    Task<TestTemplateTypeSummaryDto?> GetTemplateTypeSummaryAsync(CourseLevel courseLevel, TestType testType);
}