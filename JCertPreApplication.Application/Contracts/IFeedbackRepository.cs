using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Application.Utilities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace JCertPreApplication.Application.Contracts
{
    public interface IFeedbackRepository : IGenericRepository<Feedback>
    {
        /// <summary>
        /// Gets paginated feedbacks for a course, with optional navigation property includes (e.g., "User").
        /// </summary>
        Task<Pagination<Feedback>> GetPagingByCourseIdAsync(
            Guid courseId,
            int pageIndex,
            int pageSize,
            string includeProperties = ""
        );

        /// <summary>
        /// Gets a feedback by user and course, with optional navigation property includes (e.g., "User").
        /// </summary>
        Task<Feedback?> GetByUserAndCourseAsync(
            Guid userId,
            Guid courseId
        );

        /// <summary>
        /// Gets the average rating for a course.
        /// </summary>
        Task<decimal> GetCourseAverageRatingAsync(Guid courseId);
    }
}