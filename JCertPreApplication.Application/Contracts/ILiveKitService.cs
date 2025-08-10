using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Livekit.Server.Sdk.Dotnet;

namespace JCertPreApplication.Application.Contracts
{
    public interface ILiveKitService : IDisposable
    {
        // Token Management (Essential)
        string GenerateToken(string roomName, string participantIdentity, string participantName,
            ParticipantRole role = ParticipantRole.Student, TimeSpan? ttl = null,
            Dictionary<string, string>? attributes = null);

        // Essential Room Management
        Task<Room> CreateRoomAsync(string roomName, RoomSettings? settings = null);
        Task<Room[]> ListRoomsAsync();
        Task<Room?> GetRoomAsync(string roomName);
        Task DeleteRoomAsync(string roomName);
        Task<bool> IsRoomActiveAsync(string roomName);

        // Essential Participant Management
        Task<ParticipantInfo[]> ListParticipantsAsync(string roomName);
        Task RemoveParticipantAsync(string roomName, string identity);
        Task MuteParticipantAudioAsync(string roomName, string identity);
        Task UnmuteParticipantAudioAsync(string roomName, string identity);
    }

    // Supporting Classes và Enums
    public enum ParticipantRole
    {
        Student,
        Instructor
    }

    public class RoomSettings
    {
        public TimeSpan? EmptyTimeout { get; set; } = TimeSpan.FromMinutes(5);
        public int? MaxParticipants { get; set; } = 100;
        public string Metadata { get; set; } = string.Empty;
    }

    public class RoomStatistics
    {
        public required string RoomName { get; set; }
        public int TotalParticipants { get; set; }
        public int InstructorCount { get; set; }
        public int StudentCount { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public bool IsRecording { get; set; }
    }
} 