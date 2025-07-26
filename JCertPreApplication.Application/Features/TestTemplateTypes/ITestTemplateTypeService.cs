using JCertPreApplication.Domain.Enums;
using JCertPreApplication.Application.Utilities;

public interface ITestTemplateTypeService
{
    Task<Pagination<TestTemplateTypeDto>> GetAllAsync(string? search, CourseLevel? level, TestType? type, bool? isActive, int pageIndex, int pageSize);
    Task<TestTemplateTypeDto> CreateAsync(CreateTestTemplateTypeDto dto);
    Task<TestTemplateTypeDto> UpdateAsync(Guid testTemplateTypeId, UpdateTestTemplateTypeDto dto);
    Task DeleteAsync(Guid testTemplateTypeId);
    Task<TestTemplateTypeDto> UpdateIsActiveAsync(Guid testTemplateTypeId, bool isActive);
}