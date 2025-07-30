using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;

namespace JCertPreApplication.Application.Contracts
{
    public interface ILivestreamRepository : IGenericRepository<Livestream>
    {
        Task<Livestream?> GetLivestreamWithDetailsAsync(Guid livestreamId);
        Task<List<Livestream>> GetLivestreamsByCourseIdAsync(Guid courseId);
        Task<List<Livestream>> GetLivestreamsByUserAsync(Guid userId);
        Task<Pagination<Livestream>> GetLivestreamsWithPaginationAsync(
            Guid? courseId = null,
            string? searchTerm = null,
            int pageIndex = 1,
            int pageSize = 10);
        Task<bool> HasScheduleConflictAsync(Guid courseId, DateTime scheduledDateTime, int durationMinutes, Guid? excludeLivestreamId = null);
    }
}
