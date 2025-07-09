using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

public class SubContentService : ISubContentService
{
    private readonly IGenericRepository<SubContent> _repo;

    public SubContentService(IGenericRepository<SubContent> repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Get all SubContents with search, paging, and filtering by enums.
    /// </summary>
    public async Task<Pagination<SubContent>> GetAllAsync(string? search, CourseLevel? level, ContentName? contentName, SubContentName? subContentName, int pageIndex, int pageSize)
    {
        try
        {
            var predicate = PredicateBuilder.True<SubContent>();

            if (!string.IsNullOrWhiteSpace(search))
            {
                predicate = predicate.And(x =>
                    x.SubContentName.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    x.Level.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) ||
                    x.ContentName.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)
                );
            }
            if (level.HasValue)
            {
                predicate = predicate.And(x => x.Level == level.Value);
            }
            if (contentName.HasValue)
            {
                predicate = predicate.And(x => x.ContentName == contentName.Value);
            }
            if (subContentName.HasValue)
            {
                predicate = predicate.And(x => x.SubContentName == subContentName.Value);
            }

            return await _repo.GetPaginationAsync(predicate, null, pageIndex, pageSize);
        }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("SUBCONTENT_SERVICE_ERROR", $"Error getting subcontents: {ex.Message}");
        }
    }

    /// <summary>
    /// Create a new SubContent.
    /// </summary>
    public async Task<SubContent> CreateAsync(CreateSubContentDto dto)
    {
        try
        {
            var entity = new SubContent
            {
                SubContentId = Guid.NewGuid(),
                SubContentName = dto.SubContentName,
                Level = dto.Level,
                ContentName = dto.ContentName
            };
            await _repo.InsertAsync(entity);
            await _repo.SaveChangesAsync();
            return entity;
        }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("SUBCONTENT_SERVICE_ERROR", $"Error creating subcontent: {ex.Message}");
        }
    }

    /// <summary>
    /// Update an existing SubContent by ID.
    /// </summary>
    public async Task<SubContent> UpdateAsync(Guid subContentId, UpdateSubContentDto dto)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(subContentId);
            if (entity == null)
                throw ApiException.NotFound("SubContent", subContentId);

            // Direct assignment since SubContentName, Level, and ContentName are already of the correct type
            entity.SubContentName = dto.SubContentName;
            entity.Level = dto.Level;
            entity.ContentName = dto.ContentName;

            await _repo.UpdateAsync(entity);
            await _repo.SaveChangesAsync();
            return entity;
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("SUBCONTENT_SERVICE_ERROR", $"Error updating subcontent: {ex.Message}");
        }
    }

    /// <summary>
    /// Delete a SubContent by ID.
    /// </summary>
    public async Task DeleteAsync(Guid subContentId)
    {
        try
        {
            var entity = await _repo.GetByIdAsync(subContentId);
            if (entity == null)
                throw ApiException.NotFound("SubContent", subContentId);

            await _repo.DeleteAsync(entity);
            await _repo.SaveChangesAsync();
        }
        catch (ApiException) { throw; }
        catch (Exception ex)
        {
            throw ApiException.InternalServerError("SUBCONTENT_SERVICE_ERROR", $"Error deleting subcontent: {ex.Message}");
        }
    }
}