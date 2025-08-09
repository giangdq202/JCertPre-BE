using JCertPreApplication.Application.Dtos.TestTemplateType;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.Application.Utilities;
using System.Threading.Tasks;

public interface ITestTemplateTypeService
{
    Task<Pagination<TestTemplateTypeDto>> GetAllAsync(string? search, CourseLevel? level, TestType? type, bool? isActive, int pageIndex, int pageSize);
    Task<TestTemplateTypeDto> CreateAsync(CreateTestTemplateTypeDto dto);
    Task<TestTemplateTypeDto> UpdateAsync(Guid testTemplateTypeId, UpdateTestTemplateTypeDto dto);
    Task DeleteAsync(Guid testTemplateTypeId);
    Task<TestTemplateTypeDto> UpdateIsActiveAsync(Guid testTemplateTypeId, bool isActive);

    /// <summary>
    /// Gets summary info for a test template type and its templates.
    /// </summary>
    Task<TestTemplateTypeSummaryDto?> GetTemplateTypeSummaryAsync(CourseLevel courseLevel, TestType testType);
}