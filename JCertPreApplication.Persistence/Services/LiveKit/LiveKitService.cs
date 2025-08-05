using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Google.Protobuf;
using JCertPreApplication.Domain.Configuration;
using Livekit.Server.Sdk.Dotnet;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;

namespace JCertPreApplication.Persistence.Services.LiveKit
{
    /// <summary>
    /// Enhanced LiveKit service với đầy đủ các operations cho room, participant, data và webhook management
    /// </summary>
    public class LiveKitService : ILiveKitService
    {
        private readonly LiveKitConfiguration _config;
        private readonly RoomServiceClient _roomClient;
        private readonly WebhookReceiver _webhookReceiver;

        public LiveKitService(LiveKitConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            
            // Initialize Room Service Client
            _roomClient = new RoomServiceClient(
                _config.ServerUrl ?? "wss://your-livekit-server.com",
                _config.ApiKey,
                _config.ApiSecret
            );
            
            // Initialize Webhook Receiver
            _webhookReceiver = new WebhookReceiver(_config.ApiKey, _config.ApiSecret);
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

        /// <summary>
        /// Generate token for recording service
        /// </summary>
        public string GenerateRecordingToken(string roomName, TimeSpan? ttl = null)
        {
            var grants = new VideoGrants
            {
                RoomJoin = true,
                Room = roomName,
                RoomRecord = true,
                CanSubscribe = true,
                Hidden = true
            };

            return new AccessToken(_config.ApiKey, _config.ApiSecret)
                .WithIdentity($"recorder-{Guid.NewGuid()}")
                .WithName("Recording Service")
                .WithGrants(grants)
                .WithTtl(ttl ?? TimeSpan.FromHours(12))
                .ToJwt();
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
                MaxParticipants = (uint)(settings?.MaxParticipants ?? 100),
                Metadata = settings?.Metadata ?? string.Empty
            };

            var response = await _roomClient.CreateRoom(request);
            return response;
        }

        /// <summary>
        /// Lấy danh sách tất cả rooms
        /// </summary>
        public async Task<Room[]> ListRoomsAsync(string[]? roomNames = null)
        {
            var request = new ListRoomsRequest();
            if (roomNames?.Length > 0)
            {
                request.Names.AddRange(roomNames);
            }

            var response = await _roomClient.ListRooms(request);
            return response.Rooms.ToArray();
        }

        /// <summary>
        /// Lấy thông tin chi tiết một room
        /// </summary>
        public async Task<Room?> GetRoomAsync(string roomName)
        {
            var rooms = await ListRoomsAsync(new[] { roomName });
            return rooms.FirstOrDefault();
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
        /// Update room metadata
        /// </summary>
        public async Task<Room> UpdateRoomMetadataAsync(string roomName, string metadata)
        {
            var request = new UpdateRoomMetadataRequest
            {
                Room = roomName,
                Metadata = metadata
            };

            var response = await _roomClient.UpdateRoomMetadata(request);
            return response;
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
        /// Lấy thông tin chi tiết một participant
        /// </summary>
        public async Task<ParticipantInfo> GetParticipantAsync(string roomName, string identity)
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

            var response = await _roomClient.GetParticipant(request);
            return response;
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
        /// Update participant permissions
        /// </summary>
        public async Task<ParticipantInfo> UpdateParticipantPermissionsAsync(
            string roomName,
            string identity,
            ParticipantPermission permissions)
        {
            var request = new UpdateParticipantRequest
            {
                Room = roomName,
                Identity = identity,
                Permission = permissions
            };

            var response = await _roomClient.UpdateParticipant(request);
            return response;
        }

        /// <summary>
        /// Update participant metadata
        /// </summary>
        public async Task<ParticipantInfo> UpdateParticipantMetadataAsync(
            string roomName,
            string identity,
            string metadata)
        {
            var request = new UpdateParticipantRequest
            {
                Room = roomName,
                Identity = identity,
                Metadata = metadata
            };

            var response = await _roomClient.UpdateParticipant(request);
            return response;
        }

        /// <summary>
        /// Promote participant to instructor role
        /// </summary>
        public async Task<ParticipantInfo> PromoteToInstructorAsync(string roomName, string identity)
        {
            var permissions = new ParticipantPermission
            {
                CanSubscribe = true,
                CanPublish = true,
                CanPublishData = true
            };

            return await UpdateParticipantPermissionsAsync(roomName, identity, permissions);
        }

        /// <summary>
        /// Demote participant to student role
        /// </summary>
        public async Task<ParticipantInfo> DemoteToStudentAsync(string roomName, string identity)
        {
            var permissions = new ParticipantPermission
            {
                CanSubscribe = true,
                CanPublish = true,
                CanPublishData = false
            };

            return await UpdateParticipantPermissionsAsync(roomName, identity, permissions);
        }
        #endregion

        #region Track Management
        /// <summary>
        /// Mute/unmute participant's track
        /// </summary>
        public async Task MuteTrackAsync(
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

        /// <summary>
        /// Mute tất cả audio tracks của một participant
        /// </summary>
        public async Task MuteParticipantAudioAsync(string roomName, string identity)
        {
            var participant = await GetParticipantAsync(roomName, identity);
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
            var participant = await GetParticipantAsync(roomName, identity);
            var audioTracks = participant.Tracks
                .Where(t => t.Type == TrackType.Audio)
                .ToList();

            foreach (var track in audioTracks)
            {
                await MuteTrackAsync(roomName, identity, track.Sid, false);
            }
        }

        /// <summary>
        /// Update participant subscriptions
        /// </summary>
        public async Task UpdateSubscriptionsAsync(
            string roomName,
            string identity,
            string[] trackSids,
            bool subscribe)
        {
            var request = new UpdateSubscriptionsRequest
            {
                Room = roomName,
                Identity = identity,
                Subscribe = subscribe
            };

            request.TrackSids.AddRange(trackSids);
            await _roomClient.UpdateSubscriptions(request);
        }
        #endregion

        #region Data Management
        /// <summary>
        /// Send data message tới tất cả participants trong room
        /// </summary>
        public async Task SendDataToRoomAsync(
            string roomName,
            string data,
            DataPacket.Types.Kind kind = DataPacket.Types.Kind.Reliable)
        {
            if (string.IsNullOrEmpty(roomName))
            {
                throw ApiException.BadRequest("INVALID_ROOM_NAME", "Room name is required.");
            }

            if (string.IsNullOrEmpty(data))
            {
                throw ApiException.BadRequest("INVALID_DATA", "Data cannot be empty.");
            }

            var request = new SendDataRequest
            {
                Room = roomName,
                Data = ByteString.CopyFromUtf8(data),
                Kind = kind
            };

            await _roomClient.SendData(request);
        }

        /// <summary>
        /// Send data message tới specific participants
        /// </summary>
        public async Task SendDataToParticipantsAsync(
            string roomName,
            string data,
            string[] participantIdentities,
            DataPacket.Types.Kind kind = DataPacket.Types.Kind.Reliable)
        {
            var request = new SendDataRequest
            {
                Room = roomName,
                Data = ByteString.CopyFromUtf8(data),
                Kind = kind
            };

            request.DestinationIdentities.AddRange(participantIdentities);
            await _roomClient.SendData(request);
        }

        /// <summary>
        /// Send binary data
        /// </summary>
        public async Task SendBinaryDataAsync(
            string roomName,
            byte[] data,
            string[]? participantIdentities = null,
            DataPacket.Types.Kind kind = DataPacket.Types.Kind.Reliable)
        {
            var request = new SendDataRequest
            {
                Room = roomName,
                Data = ByteString.CopyFrom(data),
                Kind = kind
            };

            if (participantIdentities?.Length > 0)
            {
                request.DestinationIdentities.AddRange(participantIdentities);
            }

            await _roomClient.SendData(request);
        }

        /// <summary>
        /// Broadcast message tới toàn bộ room
        /// </summary>
        public async Task BroadcastMessageAsync(string roomName, object message)
        {
            var jsonData = System.Text.Json.JsonSerializer.Serialize(message);
            await SendDataToRoomAsync(roomName, jsonData, DataPacket.Types.Kind.Reliable);
        }

        /// <summary>
        /// Send control command tới instructor
        /// </summary>
        public async Task SendControlCommandAsync(
            string roomName,
            string command,
            object? payload = null)
        {
            var commandData = new
            {
                type = "control_command",
                command = command,
                payload = payload,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            var instructors = await GetInstructorsAsync(roomName);
            var instructorIdentities = instructors.Select(p => p.Identity).ToArray();

            if (instructorIdentities.Length > 0)
            {
                var jsonData = System.Text.Json.JsonSerializer.Serialize(commandData);
                await SendDataToParticipantsAsync(roomName, jsonData, instructorIdentities);
            }
        }

        /// <summary>
        /// Send emergency message từ student tới instructor (microphone issue, technical problem, etc.)
        /// </summary>
        public async Task SendEmergencyMessageAsync(
            string roomName,
            string studentIdentity,
            string studentName,
            string messageType,
            string message)
        {
            var emergencyData = new
            {
                type = "emergency_message",
                messageType = messageType, // "mic_broken", "connection_issue", "question", "help_needed"
                message = message,
                studentIdentity = studentIdentity,
                studentName = studentName,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
                priority = "high"
            };

            var instructors = await GetInstructorsAsync(roomName);
            var instructorIdentities = instructors.Select(p => p.Identity).ToArray();

            if (instructorIdentities.Length > 0)
            {
                var jsonData = System.Text.Json.JsonSerializer.Serialize(emergencyData);
                await SendDataToParticipantsAsync(roomName, jsonData, instructorIdentities);
            }
        }

        /// <summary>
        /// Send whiteboard interaction data từ student
        /// </summary>
        public async Task SendWhiteboardInteractionAsync(
            string roomName,
            string participantIdentity,
            object whiteboardData)
        {
            var interactionData = new
            {
                type = "whiteboard_interaction",
                participantIdentity = participantIdentity,
                data = whiteboardData,
                timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
            };

            var jsonData = System.Text.Json.JsonSerializer.Serialize(interactionData);
            await SendDataToRoomAsync(roomName, jsonData, DataPacket.Types.Kind.Reliable);
        }
        #endregion

        #region Webhook Processing
        /// <summary>
        /// Process incoming webhook từ LiveKit server
        /// </summary>
        public WebhookEvent ProcessWebhook(string payload, string authHeader)
        {
            try
            {
                return _webhookReceiver.Receive(payload, authHeader);
            }
            catch (Exception)
            {
                throw ApiException.InternalServerError("WEBHOOK_PROCESSING_FAILED", "Failed to process LiveKit webhook");
            }
        }

        /// <summary>
        /// Handle room events từ webhooks
        /// </summary>
        public async Task HandleRoomEventAsync(WebhookEvent webhookEvent)
        {
            switch (webhookEvent.Event)
            {
                case "room_started":
                    await OnRoomStartedAsync(webhookEvent.Room);
                    break;
                case "room_finished":
                    await OnRoomFinishedAsync(webhookEvent.Room);
                    break;
                case "participant_joined":
                    await OnParticipantJoinedAsync(webhookEvent.Room, webhookEvent.Participant);
                    break;
                case "participant_left":
                    await OnParticipantLeftAsync(webhookEvent.Room, webhookEvent.Participant);
                    break;
                case "track_published":
                    await OnTrackPublishedAsync(webhookEvent.Room, webhookEvent.Participant, webhookEvent.Track);
                    break;
                case "track_unpublished":
                    await OnTrackUnpublishedAsync(webhookEvent.Room, webhookEvent.Participant, webhookEvent.Track);
                    break;
            }
        }
        #endregion

        #region Utility Methods
        /// <summary>
        /// Get all active rooms
        /// </summary>
        public async Task<Room[]> GetActiveRoomsAsync()
        {
            var rooms = await ListRoomsAsync();
            return rooms.Where(r => r.NumParticipants > 0).ToArray();
        }

        /// <summary>
        /// Get instructors trong room
        /// </summary>
        public async Task<ParticipantInfo[]> GetInstructorsAsync(string roomName)
        {
            var participants = await ListParticipantsAsync(roomName);
            return participants
                .Where(p => p.Metadata != null && p.Metadata.Contains("\"role\":\"instructor\""))
                .ToArray();
        }

        /// <summary>
        /// Get students trong room
        /// </summary>
        public async Task<ParticipantInfo[]> GetStudentsAsync(string roomName)
        {
            var participants = await ListParticipantsAsync(roomName);
            return participants
                .Where(p => p.Metadata == null || !p.Metadata.Contains("\"role\":\"instructor\""))
                .ToArray();
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

        /// <summary>
        /// Get room statistics
        /// </summary>
        public async Task<RoomStatistics> GetRoomStatisticsAsync(string roomName)
        {
            var room = await GetRoomAsync(roomName);
            if (room == null)
            {
                throw ApiException.NotFound("Room", roomName);
            }

            var participants = await ListParticipantsAsync(roomName);

            return new RoomStatistics
            {
                RoomName = roomName,
                TotalParticipants = (int)room.NumParticipants,
                InstructorCount = participants.Count(p => p.Metadata != null && p.Metadata.Contains("\"role\":\"instructor\"")),
                StudentCount = participants.Count(p => p.Metadata == null || !p.Metadata.Contains("\"role\":\"instructor\"")),
                CreationTime = DateTimeOffset.FromUnixTimeSeconds(room.CreationTime),
                IsRecording = room.ActiveRecording
            };
        }
        #endregion

        #region Private Helper Methods
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
                    CanPublishData = true,    // ✅ Cho phép gửi tin nhắn và tương tác whiteboard
                    CanSubscribe = true,
                    CanUpdateOwnMetadata = true  // ✅ Cho phép cập nhật thông tin cá nhân
                },
                _ => throw ApiException.BadRequest("INVALID_ROLE", $"Unknown participant role: {role}")
            };
        }

        // Virtual methods for webhook event handling - có thể override trong derived classes
        protected virtual Task OnRoomStartedAsync(Room room) => Task.CompletedTask;
        protected virtual Task OnRoomFinishedAsync(Room room) => Task.CompletedTask;
        protected virtual Task OnParticipantJoinedAsync(Room room, ParticipantInfo participant) => Task.CompletedTask;
        protected virtual Task OnParticipantLeftAsync(Room room, ParticipantInfo participant) => Task.CompletedTask;
        protected virtual Task OnTrackPublishedAsync(Room room, ParticipantInfo participant, TrackInfo track) => Task.CompletedTask;
        protected virtual Task OnTrackUnpublishedAsync(Room room, ParticipantInfo participant, TrackInfo track) => Task.CompletedTask;
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
