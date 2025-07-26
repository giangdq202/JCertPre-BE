using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Enums;

public interface ITestTemplateService
{
    Task<List<TestTemplateDto>> GetAllByTypeIdAsync(Guid testTemplateTypeId);
    Task<TestTemplateDto> CreateAsync(CreateTestTemplateDto dto);
    Task<TestTemplateDto> UpdateAsync(Guid templateId, UpdateTestTemplateDto dto);
    Task DeleteAsync(Guid templateId);
}