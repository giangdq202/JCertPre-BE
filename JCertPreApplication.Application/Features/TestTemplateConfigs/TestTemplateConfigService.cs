using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.SubContent;
using JCertPreApplication.Application.Dtos.TestTemplateConfig;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Features.TestTemplateConfigs
{
    /// <summary>
    /// Service for handling business logic related to TestTemplateConfig entities.
    /// </summary>
    public class TestTemplateConfigService : ITestTemplateConfigService
    {
        private readonly ITestTemplateConfigRepository _repo;
        private readonly ITestTemplateRepository _templateRepo;
        private readonly ITestTemplateTypeRepository _typeRepo;
        private readonly IQuestionRepository _questionRepo;

        public TestTemplateConfigService(
            ITestTemplateConfigRepository repo,
            ITestTemplateRepository templateRepo,
            ITestTemplateTypeRepository typeRepo,
            IQuestionRepository questionRepo)
        {
            _repo = repo ?? throw new ArgumentNullException(nameof(repo));
            _templateRepo = templateRepo ?? throw new ArgumentNullException(nameof(templateRepo));
            _typeRepo = typeRepo ?? throw new ArgumentNullException(nameof(typeRepo));
            _questionRepo = questionRepo ?? throw new ArgumentNullException(nameof(questionRepo));
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
                var template = await _templateRepo.GetByIdAsync(templateId);
                if (template == null)
                    throw ApiException.NotFound("TestTemplate", templateId);

                var isTypeActive = await _typeRepo.AnyAsync(
                    t => t.TestTemplateTypeId == template.TestTemplateTypeId && t.isActive
                );

                if (isTypeActive)
                    throw ApiException.BadRequest("TYPE_ACTIVE", "Cannot perform this operation because the test template type is active.");

                // --- Duplicate subContentId check across all templates of the same type ---
                var allTemplates = await _templateRepo.GetAllAsync(t => t.TestTemplateTypeId == template.TestTemplateTypeId, "TestTemplateConfigs");
                var allConfigs = allTemplates.SelectMany(t => t.TestTemplateConfigs).ToList();
                if (allConfigs.Any(c => c.subContentId == dto.subContentId))
                    throw ApiException.BadRequest("DUPLICATE_SUBCONTENT", "Duplicate subContentId found in another test template of the same type.");

                // Efficiently check if there are enough questions in DB for this subContentId
                var availableCount = await _questionRepo.CountAsync(q =>
                    q.SubContentId == dto.subContentId && q.isActive);

                if (availableCount < dto.questionCount)
                    throw ApiException.BadRequest("NOT_ENOUGH_QUESTIONS",
                        $"Not enough questions in the database for subContentId {dto.subContentId}. Required: {dto.questionCount}, Available: {availableCount}");

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

                var created = await _repo.GetByConfigIdAsync(entity.configId);
                return MapToDto(created!);
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

                var template = await _templateRepo.GetByIdAsync(entity.templateId);
                if (template == null)
                    throw ApiException.NotFound("TestTemplate", entity.templateId);

                var isTypeActive = await _typeRepo.AnyAsync(
                    t => t.TestTemplateTypeId == template.TestTemplateTypeId && t.isActive
                );

                if (isTypeActive)
                    throw ApiException.BadRequest("TYPE_ACTIVE", "Cannot perform this operation because the test template type is active.");

                // Efficiently check if there are enough questions in DB for this subContentId and pointPerQuestion
                var newQuestionCount = dto.questionCount ?? entity.questionCount;
                var newPointPerQuestion = dto.pointPerQuestion ?? entity.pointPerQuestion;
                var subContentId = entity.subContentId;

                if (dto.questionCount.HasValue)
                {
                    var availableCount = await _questionRepo.CountAsync(q =>
                        q.SubContentId == subContentId && q.isActive);

                    if (availableCount < newQuestionCount)
                        throw ApiException.BadRequest("NOT_ENOUGH_QUESTIONS",
                            $"Not enough questions in the database for subContentId {subContentId}. Required: {newQuestionCount}, Available: {availableCount}");
                    entity.questionCount = dto.questionCount.Value;
                }
                if (dto.pointPerQuestion.HasValue)
                    entity.pointPerQuestion = dto.pointPerQuestion.Value;
                if (dto.totalPoints.HasValue)
                    entity.totalPoints = dto.totalPoints.Value;
                if (dto.sequence.HasValue)
                    entity.sequence = dto.sequence.Value;

                await _repo.UpdateAsync(entity);
                await _repo.SaveChangesAsync();

                var updated = await _repo.GetByConfigIdAsync(configId);
                return MapToDto(updated!);
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

                // For update/delete, use entity.templateId
                var template = await _templateRepo.GetByIdAsync(entity.templateId);
                if (template == null)
                    throw ApiException.NotFound("TestTemplate", entity.templateId);

                var isTypeActive = await _typeRepo.AnyAsync(
                    t => t.TestTemplateTypeId == template.TestTemplateTypeId && t.isActive
                );

                if (isTypeActive)
                    throw ApiException.BadRequest("TYPE_ACTIVE", "Cannot perform this operation because the test template type is active.");

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
            try
            {
                return new TestTemplateConfigDto
                {
                    configId = entity.configId,
                    templateId = entity.templateId,
                    questionCount = entity.questionCount,
                    pointPerQuestion = entity.pointPerQuestion,
                    totalPoints = entity.totalPoints,
                    sequence = entity.sequence,
                    SubContent = entity.SubContent == null ? null : new SubContentDto
                    {
                        SubContentId = entity.SubContent.SubContentId,
                        SubContentName = entity.SubContent.SubContentName.ToString(),
                        SubContentNameDescription = EnumHelper.GetEnumDescription(entity.SubContent.SubContentName),
                        Level = entity.SubContent.Level.ToString(),
                        LevelDescription = EnumHelper.GetEnumDescription(entity.SubContent.Level),
                        ContentName = entity.SubContent.ContentName.ToString(),
                        ContentNameDescription = EnumHelper.GetEnumDescription(entity.SubContent.ContentName)
                    }
                };
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError("TEST_TEMPLATE_CONFIG_ERROR", $"An error occurred while mapping TestTemplateConfig to DTO: {ex.Message}");
            }
        }
    }
}