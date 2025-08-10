using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using JCertPreApplication.Domain.Configuration;
using Livekit.Server.Sdk.Dotnet;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;

namespace JCertPreApplication.Persistence.Services.LiveKit
{
    /// <summary>
    /// Core LiveKit service for video conference - Essential operations only
    /// </summary>
    public class LiveKitService : ILiveKitService
    {
        private readonly LiveKitConfiguration _config;
        private readonly RoomServiceClient _roomClient;

        public LiveKitService(LiveKitConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            
            // Initialize Room Service Client
            _roomClient = new RoomServiceClient(
                _config.ServerUrl,
                _config.ApiKey,
                _config.ApiSecret
            );
        }

        #region Token Management
        /// <summary>
        /// Generate access token với custom permissions
        /// </summary>
        public string GenerateToken(
            string roomName,
            string participantIdentity,
            string participantName,
            ParticipantRole role = ParticipantRole.Student,
            TimeSpan? ttl = null,
            Dictionary<string, string>? attributes = null)
        {
            if (string.IsNullOrEmpty(roomName) || string.IsNullOrEmpty(participantIdentity))
            {
                throw ApiException.BadRequest("INVALID_PARAMETERS", "roomName and participantIdentity are required.");
            }

            var grants = GetVideoGrantsForRole(role, roomName);
            var tokenBuilder = new AccessToken(_config.ApiKey, _config.ApiSecret)
                .WithIdentity(participantIdentity)
                .WithName(participantName)
                .WithGrants(grants)
                .WithTtl(ttl ?? TimeSpan.FromHours(4));

            // Auto-add role to attributes for easier identification
            var allAttributes = new Dictionary<string, string>
            {
                ["role"] = role.ToString().ToLower(),
                ["joinedAt"] = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()
            };

            // Add custom attributes if provided
            if (attributes != null && attributes.Any())
            {
                foreach (var attr in attributes)
                {
                    allAttributes[attr.Key] = attr.Value;
                }
            }

            tokenBuilder.WithAttributes(allAttributes);
            return tokenBuilder.ToJwt();
        }
        #endregion

        #region Room Management
        /// <summary>
        /// Tạo room với custom settings
        /// </summary>
        public async Task<Room> CreateRoomAsync(
            string roomName,
            RoomSettings? settings = null)
        {
            if (string.IsNullOrEmpty(roomName))
            {
                throw ApiException.BadRequest("INVALID_ROOM_NAME", "Room name is required.");
            }

            var request = new CreateRoomRequest
            {
                Name = roomName,
                EmptyTimeout = (uint)(settings?.EmptyTimeout?.TotalSeconds ?? 300),
                DepartureTimeout = (uint)(settings?.DepartureTimeout?.TotalSeconds ?? 300),
                MaxParticipants = (uint)(settings?.MaxParticipants ?? 100),
                Metadata = settings?.Metadata ?? string.Empty
            };

            var response = await _roomClient.CreateRoom(request);
            return response;
        }

        /// <summary>
        /// Lấy danh sách tất cả các room đang hoạt động
        /// </summary>
        public async Task<Room[]> ListRoomsAsync()
        {
            var request = new ListRoomsRequest();
            var response = await _roomClient.ListRooms(request);
            return response.Rooms.ToArray();
        }

        /// <summary>
        /// Lấy thông tin chi tiết một room
        /// </summary>
        public async Task<Room?> GetRoomAsync(string roomName)
        {
            var request = new ListRoomsRequest();
            request.Names.Add(roomName);
            var response = await _roomClient.ListRooms(request);
            return response.Rooms.FirstOrDefault();
        }

        /// <summary>
        /// Xóa room và disconnect tất cả participants
        /// </summary>
        public async Task DeleteRoomAsync(string roomName)
        {
            if (string.IsNullOrEmpty(roomName))
            {
                throw ApiException.BadRequest("INVALID_ROOM_NAME", "Room name is required.");
            }

            var request = new DeleteRoomRequest { Room = roomName };
            await _roomClient.DeleteRoom(request);
        }

        /// <summary>
        /// Check if room exists và có participants
        /// </summary>
        public async Task<bool> IsRoomActiveAsync(string roomName)
        {
            try
            {
                var room = await GetRoomAsync(roomName);
                return room != null && room.NumParticipants > 0;
            }
            catch
            {
                return false;
            }
        }
        #endregion

        #region Participant Management
        /// <summary>
        /// Lấy danh sách participants trong room
        /// </summary>
        public async Task<ParticipantInfo[]> ListParticipantsAsync(string roomName)
        {
            var request = new ListParticipantsRequest { Room = roomName };
            var response = await _roomClient.ListParticipants(request);
            return response.Participants.ToArray();
        }

        /// <summary>
        /// Remove participant khỏi room
        /// </summary>
        public async Task RemoveParticipantAsync(string roomName, string identity)
        {
            if (string.IsNullOrEmpty(roomName) || string.IsNullOrEmpty(identity))
            {
                throw ApiException.BadRequest("INVALID_PARAMETERS", "Room name and participant identity are required.");
            }

            var request = new RoomParticipantIdentity
            {
                Room = roomName,
                Identity = identity
            };

            await _roomClient.RemoveParticipant(request);
        }

        /// <summary>
        /// Mute tất cả audio tracks của một participant
        /// </summary>
        public async Task MuteParticipantAudioAsync(string roomName, string identity)
        {
            var participants = await ListParticipantsAsync(roomName);
            var participant = participants.FirstOrDefault(p => p.Identity == identity);
            
            if (participant == null)
            {
                throw ApiException.NotFound("Participant", identity);
            }

            var audioTracks = participant.Tracks
                .Where(t => t.Type == TrackType.Audio)
                .ToList();

            foreach (var track in audioTracks)
            {
                await MuteTrackAsync(roomName, identity, track.Sid, true);
            }
        }

        /// <summary>
        /// Unmute participant audio tracks
        /// </summary>
        public async Task UnmuteParticipantAudioAsync(string roomName, string identity)
        {
            var participants = await ListParticipantsAsync(roomName);
            var participant = participants.FirstOrDefault(p => p.Identity == identity);
            
            if (participant == null)
            {
                throw ApiException.NotFound("Participant", identity);
            }

            var audioTracks = participant.Tracks
                .Where(t => t.Type == TrackType.Audio)
                .ToList();

            foreach (var track in audioTracks)
            {
                await MuteTrackAsync(roomName, identity, track.Sid, false);
            }
        }
        #endregion

        #region Private Helper Methods
        /// <summary>
        /// Mute/unmute participant's track
        /// </summary>
        private async Task MuteTrackAsync(
            string roomName,
            string identity,
            string trackSid,
            bool muted = true)
        {
            var request = new MuteRoomTrackRequest
            {
                Room = roomName,
                Identity = identity,
                TrackSid = trackSid,
                Muted = muted
            };

            await _roomClient.MutePublishedTrack(request);
        }

        private VideoGrants GetVideoGrantsForRole(ParticipantRole role, string roomName)
        {
            return role switch
            {
                ParticipantRole.Instructor => new VideoGrants
                {
                    RoomJoin = true,
                    Room = roomName,
                    CanPublish = true,
                    CanPublishData = true,
                    CanSubscribe = true,
                    CanUpdateOwnMetadata = true,
                    RoomAdmin = true  // Instructor có quyền admin trong room
                },
                ParticipantRole.Student => new VideoGrants
                {
                    RoomJoin = true,
                    Room = roomName,
                    CanPublish = true,
                    CanPublishData = true,    // ✅ Cho phép gửi tin nhắn và tương tác
                    CanSubscribe = true,
                    CanUpdateOwnMetadata = true  // ✅ Cho phép cập nhật thông tin cá nhân
                },
                _ => throw ApiException.BadRequest("INVALID_ROLE", $"Unknown participant role: {role}")
            };
        }
        #endregion

        #region IDisposable
        public void Dispose()
        {
            // LiveKit SDK client doesn't implement IDisposable
            // Add cleanup logic if needed in the future
        }
        #endregion
    }
}
