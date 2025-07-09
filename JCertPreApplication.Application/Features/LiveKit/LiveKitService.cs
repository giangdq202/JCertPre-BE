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
        private readonly LiveKitConfiguration _configuration;

        public LiveKitService(LiveKitConfiguration configuration)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            
            if (string.IsNullOrEmpty(_configuration.ApiKey))
                throw ApiException.BadRequest("LIVEKIT_CONFIG_ERROR", "LiveKit API Key is not configured");
            
            if (string.IsNullOrEmpty(_configuration.ApiSecret))
                throw ApiException.BadRequest("LIVEKIT_CONFIG_ERROR", "LiveKit API Secret is not configured");
        }

        /// <inheritdoc />
        public async Task<string> GenerateTokenAsync(
            string roomName,
            string participantIdentity,
            string? participantName = null,
            bool canPublish = true,
            bool canPublishData = true,
            bool canSubscribe = true)
        {
            if (string.IsNullOrEmpty(roomName))
                throw ApiException.BadRequest("INVALID_ROOM", "Room name cannot be empty");

            if (string.IsNullOrEmpty(participantIdentity))
                throw ApiException.BadRequest("INVALID_IDENTITY", "Participant identity cannot be empty");

            // Define participant permissions
            var grants = new VideoGrants
            {
                RoomJoin = true,
                Room = roomName,
                CanPublish = canPublish,
                CanPublishData = canPublishData,
                CanSubscribe = canSubscribe
            };

            try 
            {
                // Create token with 30-minute validity
                var tokenBuilder = new AccessToken(_configuration.ApiKey, _configuration.ApiSecret)
                    .WithIdentity(participantIdentity)
                    .WithGrants(grants)
                    .WithTtl(TimeSpan.FromMinutes(30));

                // Add display name if provided
                if (!string.IsNullOrEmpty(participantName))
                {
                    tokenBuilder.WithName(participantName);
                }

                // Generate and return the token
                return await Task.FromResult(tokenBuilder.ToJwt());
            }
            catch (Exception ex)
            {
                throw ApiException.InternalServerError(
                    "LIVEKIT_TOKEN_ERROR",
                    $"Failed to generate LiveKit token: {ex.Message}");
            }
        }
    }
} 