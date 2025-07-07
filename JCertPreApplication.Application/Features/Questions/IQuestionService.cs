using JCertPreApplication.Application.Dtos.Question;
using JCertPreApplication.Application.Utilities;

namespace JCertPreApplication.Application.Features.Questions
{
    public interface IQuestionService
    {
        Task<QuestionDto> GetByIdAsync(Guid id);
        Task<IEnumerable<QuestionDto>> GetAllAsync();
        Task<QuestionDto> CreateAsync(CreateQuestionDto createDto);
        Task<QuestionDto> UpdateAsync(Guid id, UpdateQuestionDto updateDto);
        Task DeleteAsync(Guid id);
        Task<Pagination<QuestionDto>> GetPaginatedWithDetailsAsync(string? searchTerm, int pageIndex, int pageSize);
    }
}