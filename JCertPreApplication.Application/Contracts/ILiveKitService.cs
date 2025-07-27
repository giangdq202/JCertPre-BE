using System.Threading.Tasks;

namespace JCertPreApplication.Application.Contracts
{
    public interface ILiveKitService
    {
        /// <summary>
        /// Generates a LiveKit token for room access with role-based permissions
        /// </summary>
        /// <param name="roomName">The name of the room to join</param>
        /// <param name="participantIdentity">The unique identifier for the participant</param>
        /// <param name="participantName">The display name of the participant</param>
        /// <param name="role">The role of the participant (instructor or student)</param>
        /// <returns>A JWT token for LiveKit room access</returns>
        string GenerateToken(string roomName, string participantIdentity, string participantName, string role = "student");
    }
} 