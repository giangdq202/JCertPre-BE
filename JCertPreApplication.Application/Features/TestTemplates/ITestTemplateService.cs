using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Enums;

public interface ITestTemplateService
{
    Task<Pagination<TestTemplateDto>> GetAllAsync(string? search, CourseLevel? level, TestType? type, int pageIndex, int pageSize);
    Task<TestTemplateDto> CreateAsync(CreateTestTemplateDto dto);
    Task<TestTemplateDto> UpdateAsync(Guid templateId, UpdateTestTemplateDto dto);
    Task DeleteAsync(Guid templateId);
    Task<TestTemplateDto?> GetByIdAsync(Guid templateId);
    Task<TestTemplateDto> UpdateIsActiveAsync(Guid templateId, bool isActive); // Added
}