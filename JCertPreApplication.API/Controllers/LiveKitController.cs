using Livekit.Server.Sdk.Dotnet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JCertPreApplication.Domain.Configuration;
using Microsoft.Extensions.Logging;

namespace JCertPreApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LiveKitController : ControllerBase
{
    private readonly LiveKitConfiguration _configuration;
    private readonly ILogger<LiveKitController> _logger;

    public LiveKitController(
        LiveKitConfiguration configuration,
        ILogger<LiveKitController> logger)
    {
        _configuration = configuration;
        _logger = logger;
        
        // Log configuration values when controller is instantiated
        _logger.LogInformation("LiveKit Configuration - API Key: {ApiKey}, Secret Length: {SecretLength}", 
            _configuration.ApiKey, 
            _configuration.ApiSecret?.Length ?? 0);
    }

    /// <summary>
    /// Generates a LiveKit access token for joining a room.
    /// </summary>
    /// <param name="roomName">The name of the room to join.</param>
    /// <param name="participantIdentity">The unique identifier of the participant (defaults to user's ID).</param>
    /// <param name="participantName">The display name of the participant (optional).</param>
    /// <returns>A JWT token that can be used to connect to LiveKit.</returns>
    [HttpGet("token")]
    public IActionResult GetToken(
        [FromQuery] string roomName,
        [FromQuery] string? participantIdentity = null,
        [FromQuery] string? participantName = null)
    {
        // Log the current request parameters
        _logger.LogInformation(
            "Generating token for Room: {RoomName}, Identity: {Identity}, Name: {Name}", 
            roomName, 
            participantIdentity ?? User.Identity?.Name, 
            participantName);

        // If participantIdentity is not provided, use the authenticated user's ID
        participantIdentity ??= User.Identity?.Name ?? 
            throw new UnauthorizedAccessException("User identity not found.");

        // Define participant permissions
        // By default, participants can only subscribe (view/listen) but not publish
        var grants = new VideoGrants
        {
            RoomJoin = true,
            Room = roomName,
            CanPublish = false,      // Cannot publish audio/video
            CanPublishData = false,  // Cannot send data (e.g., chat messages)
            CanSubscribe = true      // Can view/listen to others
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
            return Ok(new { token = tokenBuilder.ToJwt() });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to generate LiveKit token. API Key: {ApiKey}, Secret Length: {SecretLength}", 
                _configuration.ApiKey, 
                _configuration.ApiSecret?.Length ?? 0);
            throw;
        }
    }
} 