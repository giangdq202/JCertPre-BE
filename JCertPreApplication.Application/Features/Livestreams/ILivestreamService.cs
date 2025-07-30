using JCertPreApplication.Application.Dtos.Livestream;
using JCertPreApplication.Application.Utilities;

namespace JCertPreApplication.Application.Features.Livestreams
{
    public interface ILivestreamService
    {
        Task<LivestreamDto> CreateLivestreamAsync(CreateLivestreamDto createDto);
        Task<LivestreamDto?> GetLivestreamByIdAsync(Guid livestreamId);
        Task<LivestreamDto> UpdateLivestreamAsync(Guid livestreamId, UpdateLivestreamDto updateDto);
        Task DeleteLivestreamAsync(Guid livestreamId);
        Task<Pagination<LivestreamDto>> GetLivestreamsAsync(
            Guid? courseId = null,
            string? searchTerm = null,
            int pageIndex = 1,
            int pageSize = 10);
        Task<List<LivestreamDto>> GetLivestreamsByCourseAsync(Guid courseId);
        
        // LiveKit Integration
        Task<LivestreamJoinDto> GenerateJoinTokenAsync(Guid userId, Guid livestreamId);
        Task<bool> CanUserJoinLivestreamAsync(Guid userId, Guid livestreamId);
        Task<bool> CanInstructorStartLivestreamAsync(Guid userId, Guid livestreamId);
        Task StartLivestreamAsync(Guid livestreamId);
        Task EndLivestreamAsync(Guid livestreamId);
        
        // Utility methods
        string GetDisplayTitle(LivestreamDto livestream);
        string GetRoomName(Guid livestreamId);
    }
}
