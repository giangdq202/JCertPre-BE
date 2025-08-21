using JCertPreApplication.Application.Dtos.Question;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.Application.Features.Questions
{
    public interface IQuestionService
    {
        Task<QuestionDto> GetByIdAsync(Guid id);
        Task<QuestionDto> CreateAsync(CreateQuestionDto createDto);
        Task<QuestionDto> UpdateAsync(Guid id, UpdateQuestionDto updateDto);
        Task DeleteAsync(Guid id);
        Task<Pagination<QuestionDto>> GetPaginatedWithDetailsAsync(
            string? searchTerm,
            int pageIndex,
            int pageSize,
            ContentName? contentName = null,
            CourseLevel? level = null,
            SubContentName? subContentName = null,
            QuestionDifficulty? difficulty = null
        );
        Task<Pagination<QuestionDto>> GetPaginatedActiveWithDetailsAsync(
            string? searchTerm,
            int pageIndex,
            int pageSize,
            ContentName? contentName = null,
            CourseLevel? level = null,
            SubContentName? subContentName = null,
            QuestionDifficulty? difficulty = null
);
        Task<QuestionForTestDto?> GetByIdForTestAsync(Guid id);
        Task<ImportQuestionsResultDto> ImportQuestionsAsync(ImportQuestionsRequestDto dto);
        Task<GeneratedQuestionResponseDto> GenerateQuestionWithAIAsync(GenerateQuestionRequestDto requestDto);
        Task<ExplanationResponseDto> GenerateExplanationAsync(ExplanationRequestDto requestDto);
    }
}