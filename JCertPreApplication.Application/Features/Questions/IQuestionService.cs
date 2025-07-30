using JCertPreApplication.Application.Dtos.Question;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using System;

namespace JCertPreApplication.Application.Features.Questions
{
    public interface IQuestionService
    {
        Task<QuestionDto> GetByIdAsync(Guid id);
        Task<IEnumerable<QuestionDto>> GetAllAsync();
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
    }
}