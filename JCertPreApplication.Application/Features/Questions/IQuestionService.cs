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
        Task<IEnumerable<QuestionDto>> GetQuestionsWithDetailsAsync();
        Task<Pagination<QuestionDto>> GetPaginatedAsync(
            string? searchTerm = null,
            bool includeChoices = false,
            int pageIndex = 1,
            int pageSize = 10);
    }
}