using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.TestTemplateConfig;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Features.TestTemplateConfigs
{
    /// <summary>
    /// Service for handling business logic related to TestTemplateConfig entities.
    /// </summary>
    public class TestTemplateConfigService : ITestTemplateConfigService
    {
        private readonly ITestTemplateConfigRepository _repo;

        public TestTemplateConfigService(ITestTemplateConfigRepository repo)
        {
            _repo = repo;
        }

        public async Task<List<TestTemplateConfigDto>> GetAllByTemplateIdAsync(Guid templateId)
        {
            try
            {
                var configs = await _repo.GetAllByTemplateIdAsync(templateId);
                return configs.Select(MapToDto).ToList();
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("GET_TEST_TEMPLATE_CONFIGS_ERROR", ex.Message);
            }
        }

        public async Task<TestTemplateConfigDto?> GetByConfigIdAsync(Guid configId)
        {
            try
            {
                var config = await _repo.GetByConfigIdAsync(configId);
                return config == null ? null : MapToDto(config);
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("GET_TEST_TEMPLATE_CONFIG_ERROR", ex.Message);
            }
        }

        public async Task<TestTemplateConfigDto> CreateAsync(Guid templateId, CreateTestTemplateConfigDto dto)
        {
            try
            {
                var entity = new TestTemplateConfig
                {
                    configId = Guid.NewGuid(),
                    templateId = templateId,
                    subContentId = dto.subContentId,
                    questionCount = dto.questionCount,
                    pointPerQuestion = dto.pointPerQuestion,
                    totalPoints = dto.totalPoints,
                    sequence = dto.sequence
                };
                await _repo.InsertAsync(entity);
                await _repo.SaveChangesAsync();
                return MapToDto(entity);
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("CREATE_TEST_TEMPLATE_CONFIG_ERROR", ex.Message);
            }
        }

        public async Task<TestTemplateConfigDto> UpdateAsync(Guid configId, UpdateTestTemplateConfigDto dto)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(configId);
                if (entity == null)
                    throw ApiException.NotFound("TestTemplateConfig", configId);

                if (dto.questionCount.HasValue)
                    entity.questionCount = dto.questionCount.Value;
                if (dto.pointPerQuestion.HasValue)
                    entity.pointPerQuestion = dto.pointPerQuestion.Value;
                if (dto.totalPoints.HasValue)
                    entity.totalPoints = dto.totalPoints.Value;
                if (dto.sequence.HasValue)
                    entity.sequence = dto.sequence.Value;

                await _repo.UpdateAsync(entity);
                await _repo.SaveChangesAsync();
                return MapToDto(entity);
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("UPDATE_TEST_TEMPLATE_CONFIG_ERROR", ex.Message);
            }
        }

        public async Task DeleteAsync(Guid configId)
        {
            try
            {
                var entity = await _repo.GetByIdAsync(configId);
                if (entity == null)
                    throw ApiException.NotFound("TestTemplateConfig", configId);

                await _repo.DeleteAsync(entity);
                await _repo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("DELETE_TEST_TEMPLATE_CONFIG_ERROR", ex.Message);
            }
        }

        private static TestTemplateConfigDto MapToDto(TestTemplateConfig entity)
        {
            return new TestTemplateConfigDto
            {
                configId = entity.configId,
                templateId = entity.templateId,
                subContentId = entity.subContentId,
                questionCount = entity.questionCount,
                pointPerQuestion = entity.pointPerQuestion,
                totalPoints = entity.totalPoints,
                sequence = entity.sequence,
                subContentName = entity.SubContent?.SubContentName.ToString()
            };
        }
    }
}