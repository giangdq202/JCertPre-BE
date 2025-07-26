using JCertPreApplication.Application.Dtos.TestTemplateConfig;

namespace JCertPreApplication.Application.Features.TestTemplateConfigs
{
    public interface ITestTemplateConfigService
    {
        Task<List<TestTemplateConfigDto>> GetAllByTemplateIdAsync(Guid templateId);
        Task<TestTemplateConfigDto?> GetByConfigIdAsync(Guid configId);
        Task<TestTemplateConfigDto> CreateAsync(Guid templateId, CreateTestTemplateConfigDto dto);
        Task<TestTemplateConfigDto> UpdateAsync(Guid configId, UpdateTestTemplateConfigDto dto);
        Task DeleteAsync(Guid configId);
    }
}