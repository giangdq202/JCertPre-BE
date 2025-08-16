using JCertPreApplication.Application.Dtos.Feedback;
using JCertPreApplication.Application.Utilities;
using System;
using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.Feedbacks
{
    public interface IFeedbackService
    {
        Task<Pagination<FeedbackDto>> GetPagingByCourseIdAsync(Guid courseId, int pageIndex, int pageSize);
        Task<FeedbackDto> CreateAsync(CreateFeedbackDto dto);
        Task<FeedbackDto> UpdateAsync(Guid userId, Guid courseId, UpdateFeedbackDto dto);
        Task DeleteAsync(Guid userId, Guid courseId);
        Task<decimal> GetCourseAverageRatingAsync(Guid courseId);
    }
}