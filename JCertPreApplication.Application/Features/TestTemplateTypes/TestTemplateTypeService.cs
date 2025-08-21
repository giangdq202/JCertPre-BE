using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.TestTemplateType;
using JCertPreApplication.Application.Dtos.TestTemplateTypes;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using System.Linq.Expressions;
namespace JCertPreApplication.Application.Features.TestTemplateTypes;
/// <summary>
/// Service for handling business logic related to TestTemplateType entities.
/// Implements exception handling and follows Clean Architecture best practices.
/// </summary>
public class TestTemplateTypeService : ITestTemplateTypeService
{
    private readonly ITestTemplateTypeRepository _repo;
    private readonly ITestRepository _testRepository;
    private readonly IGenericRepository<TestTemplate> _testTemplateRepository;
    private readonly IGenericRepository<TestTemplateConfig> _testTemplateConfigRepository;

    public TestTemplateTypeService(
        ITestTemplateTypeRepository repo,
        ITestRepository testRepository,
        IGenericRepository<TestTemplate> testTemplateRepository,
        IGenericRepository<TestTemplateConfig> testTemplateConfigRepository)
    {
        _repo = repo ?? throw new ArgumentNullException(nameof(repo));
        _testRepository = testRepository ?? throw new ArgumentNullException(nameof(testRepository));
        _testTemplateRepository = testTemplateRepository ?? throw new ArgumentNullException(nameof(testTemplateRepository));
        _testTemplateConfigRepository = testTemplateConfigRepository ?? throw new ArgumentNullException(nameof(testTemplateConfigRepository));
    }

    /// <summary>
    /// Get all test template types with search, filter, and paging.
    /// </summary>
    public async Task<Pagination<TestTemplateTypeDto>> GetAllAsync(string? search, CourseLevel? level, TestType? type, bool? isActive, int pageIndex, int pageSize)
    {
        try
        {
            Expression<Func<TestTemplateType, bool>>? predicate = null;
            if (!string.IsNullOrWhiteSpace(search) || level.HasValue || type.HasValue || isActive.HasValue)
            {
                predicate = t =>
                    (string.IsNullOrEmpty(search) || t.typeName.ToLower().Contains(search.ToLower()))
                    && (!level.HasValue || t.courseLevel == level.Value)
                    && (!type.HasValue || t.testType == type.Value)
                    && (!isActive.HasValue || t.isActive == isActive.Value);
            }

            // Eager load CreatedByUser and VerifiedByUser
            var paginated = await _repo.GetPaginationAsync(
                predicate,
                includeProperties: "CreatedByUser,VerifiedByUser",
                pageIndex,
                pageSize,
                orderBy: q => q.OrderByDescending(t => t.createdAt)
            );

            return new Pagination<TestTemplateTypeDto>
            {
                PageIndex = paginated.PageIndex,
                PageSize = paginated.PageSize,
                TotalItemsCount = paginated.TotalItemsCount,
                Items = paginated.Items.Select(MapToDto).ToList()
            };
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("GET_TEST_TEMPLATE_TYPE_ERROR", ex.Message);
        }
    }

    /// <summary>
    /// Create a new test template type.
    /// </summary>
    public async Task<TestTemplateTypeDto> CreateAsync(CreateTestTemplateTypeDto dto)
    {
        try
        {
            // Check for existing testType and courseLevel
            var existing = await _repo.GetFirstOrDefaultAsync(
                t => t.testType == dto.testType && t.courseLevel == dto.courseLevel
            );
            if (existing != null)
                throw ApiException.BadRequest("DUPLICATE_TEST_TEMPLATE_TYPE", "A test template type with the same test type and course level already exists.");

            var entity = new TestTemplateType
            {
                TestTemplateTypeId = Guid.NewGuid(),
                userId = dto.userId,
                typeName = dto.typeName,
                courseLevel = dto.courseLevel,
                testType = dto.testType,
                description = dto.description,
                isActive = false,
                createdAt = DateTime.UtcNow,
                totalTestScore = dto.totalTestScore,
                totalPassPercentage = dto.totalPassPercentage,
            };
            await _repo.InsertAsync(entity);
            await _repo.SaveChangesAsync();
            return MapToDto(entity);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("CREATE_TEST_TEMPLATE_TYPE_ERROR", ex.Message);
        }
    }

    /// <summary>
    /// Update a test template type by id. Only updates provided fields.
    /// </summary>
    public async Task<TestTemplateTypeDto> UpdateAsync(Guid testTemplateTypeId, UpdateTestTemplateTypeDto dto)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(testTemplateTypeId);
            if (entity == null)
                throw ApiException.NotFound("TestTemplateType", testTemplateTypeId);

            // Determine the new values to check for duplicates
            var newTestType = dto.testType ?? entity.testType;
            var newCourseLevel = dto.courseLevel ?? entity.courseLevel;

            // Check for existing testType and courseLevel (excluding current entity)
            var existing = await _repo.GetFirstOrDefaultAsync(
                t => t.testType == newTestType
                    && t.courseLevel == newCourseLevel
                    && t.TestTemplateTypeId != testTemplateTypeId
            );
            if (existing != null)
                throw ApiException.BadRequest("DUPLICATE_TEST_TEMPLATE_TYPE", "A test template type with the same test type and course level already exists.");

            if (dto.typeName != null)
                entity.typeName = dto.typeName;
            if (dto.courseLevel.HasValue)
                entity.courseLevel = dto.courseLevel.Value;
            if (dto.testType.HasValue)
                entity.testType = dto.testType.Value;
            if (dto.description != null)
                entity.description = dto.description;
            if (dto.isActive.HasValue)
                entity.isActive = dto.isActive.Value;
            if (dto.totalTestScore.HasValue)
                entity.totalTestScore = dto.totalTestScore.Value;
            if (dto.totalPassPercentage.HasValue)
                entity.totalPassPercentage = dto.totalPassPercentage.Value;

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
            return MapToDto(entity);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("UPDATE_TEST_TEMPLATE_TYPE_ERROR", ex.Message);
        }
    }

    /// <summary>
    /// Delete a test template type by id.
    /// </summary>
    public async Task DeleteAsync(Guid testTemplateTypeId)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(testTemplateTypeId);
            if (entity == null)
                throw ApiException.NotFound("TestTemplateType", testTemplateTypeId);

            // Prevent deletion if active and used in any open test
            if (entity.isActive)
            {
                var existsInOpenTest = await _testRepository.AnyAsync(
                    t => t.TestTemplateTypeId == testTemplateTypeId && t.status == TestStatus.Open
                );
                if (existsInOpenTest)
                    throw ApiException.BadRequest("DELETE_NOT_ALLOWED", "Cannot delete an active template type that is used in any open test.");
            }

            await _repo.DeleteAsync(entity);
            await _repo.SaveChangesAsync();
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("DELETE_TEST_TEMPLATE_TYPE_ERROR", ex.Message);
        }
    }

    /// <summary>
    /// Update only the isActive field of a test template type by id.
    /// </summary>
    public async Task<TestTemplateTypeDto> UpdateIsActiveAsync(Guid testTemplateTypeId, bool isActive)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(testTemplateTypeId);
            if (entity == null)
                throw ApiException.NotFound("TestTemplateType", testTemplateTypeId);

            if (isActive)
            {
                // Only allow activation if verified
                if (!entity.verifiedUserId.HasValue)
                    throw ApiException.BadRequest("NOT_VERIFIED", "Cannot activate: This template type is not verified.");


                // Check if any TestTemplate exists for this type
                var hasTestTemplate = await _testTemplateRepository.AnyAsync(
                    t => t.TestTemplateTypeId == testTemplateTypeId
                );
                if (!hasTestTemplate)
                    throw ApiException.BadRequest("NO_TEST_TEMPLATE", "Cannot activate: No test template belongs to this type.");


                // With the following line:
                var testTemplatesOfType = await _testTemplateRepository.GetAllAsync(t => t.TestTemplateTypeId == testTemplateTypeId);

                var templateIds = testTemplatesOfType.Select(t => t.templateId).ToList();

                var hasConfig = await _testTemplateConfigRepository.AnyAsync(
                    c => templateIds.Contains(c.templateId)
                );

                if (!hasConfig)
                    throw ApiException.BadRequest("NO_TEST_TEMPLATE_CONFIG", "Cannot activate: No test template config belongs to any test template of this type.");
            }

            entity.isActive = isActive;
            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
            return MapToDto(entity);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("UPDATE_TEST_TEMPLATE_TYPE_ISACTIVE_ERROR", ex.Message);
        }
    }

    /// <summary>
    /// Get a summary of a test template type, including its test templates and configuration.
    /// </summary>
    public async Task<TestTemplateTypeSummaryDto?> GetTemplateTypeSummaryAsync(CourseLevel courseLevel, TestType testType)
    {
        try
        {
            var templateType = await _repo.GetFirstOrDefaultAsync(
                t => t.courseLevel == courseLevel && t.testType == testType
            );
            if (templateType == null)
                return null;

            var templatesQuery = await _testTemplateRepository.GetAll();
            var templates = templatesQuery
                .Where(t => t.TestTemplateTypeId == templateType.TestTemplateTypeId)
                .Select(t => new
                {
                    t.templateId,
                    t.templateName,
                    t.totalScore,
                    t.toPassPercentage,
                    t.durationMinutes
                })
                .ToList();

            var templateIds = templates.Select(t => t.templateId).ToList();

            var configsQuery = await _testTemplateConfigRepository.GetAll();
            var configs = configsQuery
                .Where(c => templateIds.Contains(c.templateId))
                .Select(c => new { c.templateId, c.questionCount })
                .ToList();

            var questionCountMap = configs
                .GroupBy(c => c.templateId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.questionCount));

            var result = new TestTemplateTypeSummaryDto
            {
                TestTemplateTypeId = templateType.TestTemplateTypeId,
                TypeName = templateType.typeName,
                CourseLevel = templateType.courseLevel,
                TestType = templateType.testType,
                TotalTestScore = templateType.totalTestScore,
                TotalPassPercentage = templateType.totalPassPercentage,
                TotalDurationMinutes = templates.Sum(t => t.durationMinutes),
                TestTemplates = templates.Select(t => new TestTemplateSummaryDto
                {
                    TemplateId = t.templateId,
                    TemplateName = t.templateName,
                    TotalScore = t.totalScore,
                    ToPassPercentage = t.toPassPercentage,
                    DurationMinutes = t.durationMinutes,
                    TotalQuestionCount = questionCountMap.TryGetValue(t.templateId, out var count) ? count : 0
                }).OrderBy(t => t.TemplateName).ToList()
            };

            return result;
        }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("GET_TEMPLATE_TYPE_SUMMARY_ERROR", ex.Message);
        }
    }
    /// <summary>
    /// Verify a test template type by id, associating it with a user.
    /// </summary>
    public async Task<TestTemplateTypeDto> VerifyAsync(Guid testTemplateTypeId, Guid userId)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(testTemplateTypeId);
            if (entity == null)
                throw ApiException.NotFound("TestTemplateType", testTemplateTypeId);

            // Prevent self-verification
            if (entity.userId == userId)
                throw ApiException.BadRequest("SELF_VERIFY_NOT_ALLOWED", "The creator cannot verify their own template type.");

            entity.verifiedUserId = userId;
            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
            return MapToDto(entity);
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("VERIFY_TEST_TEMPLATE_TYPE_ERROR", ex.Message);
        }
    }

    private static TestTemplateTypeDto MapToDto(TestTemplateType t) => new()
    {
        TestTemplateTypeId = t.TestTemplateTypeId,
        userId = t.userId,
        CreatedByUserName = t.CreatedByUser?.fullName, // NEW
        verifiedUserId = t.verifiedUserId,
        VerifiedByUserName = t.VerifiedByUser?.fullName, // NEW
        typeName = t.typeName,
        courseLevel = t.courseLevel,
        testType = t.testType,
        description = t.description,
        isActive = t.isActive,
        createdAt = t.createdAt,
        totalTestScore = t.totalTestScore,
        totalPassPercentage = t.totalPassPercentage,
    };

    
}