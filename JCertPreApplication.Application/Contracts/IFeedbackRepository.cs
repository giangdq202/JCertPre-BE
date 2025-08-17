using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Application.Utilities;
using System;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace JCertPreApplication.Application.Contracts
{
    public interface IFeedbackRepository : IGenericRepository<Feedback>
    {
        Task<Pagination<Feedback>> GetPagingByCourseIdAsync(Guid courseId, int pageIndex, int pageSize);
        Task<Feedback?> GetByUserAndCourseAsync(Guid userId, Guid courseId);
        Task<decimal> GetCourseAverageRatingAsync(Guid courseId);
    }
}