using JCertPreApplication.Application.Dtos.Question;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Features.Questions
{
    public interface IQuestionService
    {
        Task<Question> GetByIdAsync(Guid id);
        Task<IEnumerable<Question>> GetAllAsync();
        Task<Question> CreateAsync(CreateQuestionDto createDto, ContentName contentName, CourseLevel level, SubContentName subContentName);
        Task<Question> UpdateAsync(Guid id, UpdateQuestionDto updateDto);
        Task DeleteAsync(Guid id);
        Task<Pagination<Question>> GetPaginatedWithDetailsAsync(
            string? searchTerm, 
            int pageIndex, 
            int pageSize,
            ContentName? contentName = null,
            CourseLevel? level = null,
            SubContentName? subContentName = null
        );
    }
}