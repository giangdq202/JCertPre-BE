using System;
using System.Threading.Tasks;
using JCertPreApplication.Domain.Configuration;
using Livekit.Server.Sdk.Dotnet;
using JCertPreApplication.Application.Exceptions;
using System.Collections.Generic;

namespace JCertPreApplication.Application.Features.LiveKit
{
    /// <summary>
    /// Implementation of LiveKit service operations
    /// </summary>
    public class LiveKitService : ILiveKitService
    {
        private readonly LiveKitConfiguration _liveKitConfig;

        public LiveKitService(LiveKitConfiguration liveKitConfig)
        {
            _liveKitConfig = liveKitConfig;
        }

        public string GenerateToken(string roomName, string participantIdentity, string participantName, string role = "student")
        {
            if (string.IsNullOrEmpty(roomName) || string.IsNullOrEmpty(participantIdentity))
            {
                throw new ArgumentException("roomName and participantIdentity are required.");
            }

            VideoGrants grants;

            switch (role.ToLower())
            {
                case "instructor":
                    // Instructors have full permissions: video, audio, data, and screen sharing
                    grants = new VideoGrants
                    {
                        RoomJoin = true,
                        Room = roomName,
                        CanPublish = true,       // Can publish all sources
                        CanPublishData = true,   // Critical: for sending control commands and whiteboard data
                        CanSubscribe = true,
                        CanUpdateOwnMetadata = true
                    };
                    break;

                case "student":
                default:
                    // Students have limited permissions
                    grants = new VideoGrants
                    {
                        RoomJoin = true,
                        Room = roomName,
                        CanPublish = true,  // Allow publish, but client will only allow audio
                        CanPublishData = false, // Prevent sending data to avoid command spam
                        CanSubscribe = true,    // Required to see/hear the instructor
                        Hidden = true // Hide students from other students' participant list (optional)
                    };
                    break;
            }

            var tokenBuilder = new AccessToken(_liveKitConfig.ApiKey, _liveKitConfig.ApiSecret)
                .WithIdentity(participantIdentity)
                .WithName(participantName)
                .WithGrants(grants)
                .WithTtl(TimeSpan.FromHours(4)); // Duration for one class session

            return tokenBuilder.ToJwt();
        }
    }
} 