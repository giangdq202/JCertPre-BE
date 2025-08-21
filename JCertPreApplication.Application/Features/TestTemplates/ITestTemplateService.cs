using JCertPreApplication.Application.Dtos.TestTemplate;

namespace JCertPreApplication.Application.Features.TestTemplates;
public interface ITestTemplateService
{
    Task<List<TestTemplateDto>> GetAllByTypeIdAsync(Guid testTemplateTypeId);
    Task<TestTemplateDto> CreateAsync(CreateTestTemplateDto dto);
    Task<TestTemplateDto> UpdateAsync(Guid templateId, UpdateTestTemplateDto dto);
    Task DeleteAsync(Guid templateId);
}