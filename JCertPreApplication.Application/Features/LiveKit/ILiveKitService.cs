using System.Threading.Tasks;

namespace JCertPreApplication.Application.Features.LiveKit
{
    /// <summary>
    /// Interface for LiveKit service operations
    /// </summary>
    public interface ILiveKitService
    {
        /// <summary>
        /// Generates a LiveKit access token for joining a room
        /// </summary>
        /// <param name="roomName">The name of the room to join</param>
        /// <param name="participantIdentity">The unique identifier of the participant</param>
        /// <param name="participantName">The display name of the participant (optional)</param>
        /// <param name="canPublish">Whether the participant can publish audio/video</param>
        /// <param name="canPublishData">Whether the participant can send data</param>
        /// <param name="canSubscribe">Whether the participant can subscribe to others</param>
        /// <returns>A JWT token that can be used to connect to LiveKit</returns>
        Task<string> GenerateTokenAsync(
            string roomName,
            string participantIdentity,
            string? participantName = null,
            bool canPublish = true,
            bool canPublishData = true,
            bool canSubscribe = true);
    }
} 