using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface ITestTemplateConfigRepository : IGenericRepository<TestTemplateConfig>
    {
        Task<List<TestTemplateConfig>> GetAllByTemplateIdAsync(Guid templateId);
        Task<TestTemplateConfig?> GetByConfigIdAsync(Guid configId);
    }
}