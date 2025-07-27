using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Livekit.Server.Sdk.Dotnet;

namespace JCertPreApplication.Application.Contracts
{
    public interface ILiveKitService : IDisposable
    {
        // Token Management
        string GenerateToken(string roomName, string participantIdentity, string participantName,
            ParticipantRole role = ParticipantRole.Student, TimeSpan? ttl = null,
            Dictionary<string, string>? attributes = null);
        string GenerateAdminToken(string roomName, string participantIdentity, string participantName, TimeSpan? ttl = null);
        string GenerateRecordingToken(string roomName, TimeSpan? ttl = null);

        // Room Management
        Task<Room> CreateRoomAsync(string roomName, RoomSettings? settings = null);
        Task<Room[]> ListRoomsAsync(string[]? roomNames = null);
        Task<Room?> GetRoomAsync(string roomName);
        Task DeleteRoomAsync(string roomName);
        Task<Room> UpdateRoomMetadataAsync(string roomName, string metadata);

        // Participant Management
        Task<ParticipantInfo[]> ListParticipantsAsync(string roomName);
        Task<ParticipantInfo> GetParticipantAsync(string roomName, string identity);
        Task RemoveParticipantAsync(string roomName, string identity);
        Task<ParticipantInfo> UpdateParticipantPermissionsAsync(string roomName, string identity, ParticipantPermission permissions);
        Task<ParticipantInfo> UpdateParticipantMetadataAsync(string roomName, string identity, string metadata);
        Task<ParticipantInfo> PromoteToInstructorAsync(string roomName, string identity);
        Task<ParticipantInfo> DemoteToStudentAsync(string roomName, string identity);

        // Track Management
        Task MuteTrackAsync(string roomName, string identity, string trackSid, bool muted = true);
        Task MuteParticipantAudioAsync(string roomName, string identity);
        Task UpdateSubscriptionsAsync(string roomName, string identity, string[] trackSids, bool subscribe);

        // Data Management
        Task SendDataToRoomAsync(string roomName, string data, DataPacket.Types.Kind kind = DataPacket.Types.Kind.Reliable);
        Task SendDataToParticipantsAsync(string roomName, string data, string[] participantIdentities, DataPacket.Types.Kind kind = DataPacket.Types.Kind.Reliable);
        Task SendBinaryDataAsync(string roomName, byte[] data, string[]? participantIdentities = null, DataPacket.Types.Kind kind = DataPacket.Types.Kind.Reliable);
        Task BroadcastMessageAsync(string roomName, object message);
        Task SendControlCommandAsync(string roomName, string command, object? payload = null);

        // Webhook Processing
        WebhookEvent ProcessWebhook(string payload, string authHeader);
        Task HandleRoomEventAsync(WebhookEvent webhookEvent);

        // Utility Methods
        Task<Room[]> GetActiveRoomsAsync();
        Task<ParticipantInfo[]> GetInstructorsAsync(string roomName);
        Task<ParticipantInfo[]> GetStudentsAsync(string roomName);
        Task<bool> IsRoomActiveAsync(string roomName);
        Task<RoomStatistics> GetRoomStatisticsAsync(string roomName);
    }

    // Supporting Classes và Enums
    public enum ParticipantRole
    {
        Student,
        Instructor,
        Admin
    }

    public class RoomSettings
    {
        public TimeSpan? EmptyTimeout { get; set; } = TimeSpan.FromMinutes(5);
        public int? MaxParticipants { get; set; } = 100;
        public string Metadata { get; set; } = string.Empty;
    }

    public class RoomStatistics
    {
        public string RoomName { get; set; }
        public int TotalParticipants { get; set; }
        public int InstructorCount { get; set; }
        public int StudentCount { get; set; }
        public DateTimeOffset CreationTime { get; set; }
        public bool IsRecording { get; set; }
    }
} 