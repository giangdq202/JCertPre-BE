using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using System.ComponentModel.DataAnnotations;

namespace JCertPreApplication.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LiveKitController : ControllerBase
{
    private readonly ILiveKitService _liveKitService;

    public LiveKitController(ILiveKitService liveKitService)
    {
        _liveKitService = liveKitService;
    }

    /// <summary>
    /// Generates a LiveKit access token for joining a room with role-based permissions.
    /// </summary>
    /// <param name="roomName">The name of the room to join.</param>
    /// <param name="participantIdentity">The unique identifier of the participant (defaults to user's ID).</param>
    /// <param name="participantName">The display name of the participant (optional).</param>
    /// <param name="role">The role of the participant (Student, Instructor, Admin).</param>
    /// <returns>A JWT token that can be used to connect to LiveKit.</returns>
    [HttpGet("token")]
    public IActionResult GetToken(
        [FromQuery] [Required] string roomName,
        [FromQuery] string? participantIdentity = null,
        [FromQuery] string? participantName = null,
        [FromQuery] ParticipantRole role = ParticipantRole.Student)
    {
        if (string.IsNullOrEmpty(roomName))
        {
            return BadRequest("roomName is required.");
        }

        // If participantIdentity is not provided, use the authenticated user's ID
        participantIdentity ??= User.Identity?.Name ?? 
            throw ApiException.Unauthorized("User identity not found");

        // If participantName is not provided, use the participantIdentity
        participantName ??= participantIdentity;

        var token = _liveKitService.GenerateToken(
            roomName,
            participantIdentity,
            participantName,
            role);

        return Ok(new { token });
    }

    /// <summary>
    /// Generates an admin token with full permissions.
    /// </summary>
    [HttpGet("admin-token")]
    public IActionResult GetAdminToken(
        [FromQuery] [Required] string roomName,
        [FromQuery] string? participantIdentity = null,
        [FromQuery] string? participantName = null)
    {
        participantIdentity ??= User.Identity?.Name ?? 
            throw ApiException.Unauthorized("User identity not found");
        participantName ??= participantIdentity;

        var token = _liveKitService.GenerateAdminToken(roomName, participantIdentity, participantName);

        return Ok(new { token });
    }

    /// <summary>
    /// Creates a new room with specified settings.
    /// </summary>
    [HttpPost("rooms")]
    public async Task<IActionResult> CreateRoom(
        [FromBody] CreateRoomRequest request)
    {
        var settings = new RoomSettings
        {
            EmptyTimeout = TimeSpan.FromMinutes(request.EmptyTimeoutMinutes ?? 5),
            MaxParticipants = request.MaxParticipants ?? 100,
            Metadata = request.Metadata ?? string.Empty
        };

        var room = await _liveKitService.CreateRoomAsync(request.RoomName, settings);
        return Ok(room);
    }

    /// <summary>
    /// Gets all active rooms.
    /// </summary>
    [HttpGet("rooms")]
    public async Task<IActionResult> GetRooms()
    {
        var rooms = await _liveKitService.ListRoomsAsync();
        return Ok(rooms);
    }

    /// <summary>
    /// Gets information about a specific room.
    /// </summary>
    [HttpGet("rooms/{roomName}")]
    public async Task<IActionResult> GetRoom(string roomName)
    {
        var room = await _liveKitService.GetRoomAsync(roomName);
        if (room == null)
        {
            return NotFound($"Room '{roomName}' not found");
        }
        return Ok(room);
    }

    /// <summary>
    /// Deletes a room and disconnects all participants.
    /// </summary>
    [HttpDelete("rooms/{roomName}")]
    public async Task<IActionResult> DeleteRoom(string roomName)
    {
        await _liveKitService.DeleteRoomAsync(roomName);
        return NoContent();
    }

    /// <summary>
    /// Gets all participants in a room.
    /// </summary>
    [HttpGet("rooms/{roomName}/participants")]
    public async Task<IActionResult> GetParticipants(string roomName)
    {
        var participants = await _liveKitService.ListParticipantsAsync(roomName);
        return Ok(participants);
    }

    /// <summary>
    /// Removes a participant from a room.
    /// </summary>
    [HttpDelete("rooms/{roomName}/participants/{identity}")]
    public async Task<IActionResult> RemoveParticipant(string roomName, string identity)
    {
        await _liveKitService.RemoveParticipantAsync(roomName, identity);
        return NoContent();
    }

    /// <summary>
    /// Promotes a participant to instructor role.
    /// </summary>
    [HttpPost("rooms/{roomName}/participants/{identity}/promote")]
    public async Task<IActionResult> PromoteToInstructor(string roomName, string identity)
    {
        var participant = await _liveKitService.PromoteToInstructorAsync(roomName, identity);
        return Ok(participant);
    }

    /// <summary>
    /// Demotes a participant to student role.
    /// </summary>
    [HttpPost("rooms/{roomName}/participants/{identity}/demote")]
    public async Task<IActionResult> DemoteToStudent(string roomName, string identity)
    {
        var participant = await _liveKitService.DemoteToStudentAsync(roomName, identity);
        return Ok(participant);
    }

    /// <summary>
    /// Mutes a participant's audio.
    /// </summary>
    [HttpPost("rooms/{roomName}/participants/{identity}/mute")]
    public async Task<IActionResult> MuteParticipant(string roomName, string identity)
    {
        await _liveKitService.MuteParticipantAudioAsync(roomName, identity);
        return NoContent();
    }

    /// <summary>
    /// Sends a message to all participants in a room.
    /// </summary>
    [HttpPost("rooms/{roomName}/broadcast")]
    public async Task<IActionResult> BroadcastMessage(
        string roomName,
        [FromBody] BroadcastMessageRequest request)
    {
        await _liveKitService.BroadcastMessageAsync(roomName, request.Message);
        return NoContent();
    }

    /// <summary>
    /// Gets room statistics.
    /// </summary>
    [HttpGet("rooms/{roomName}/statistics")]
    public async Task<IActionResult> GetRoomStatistics(string roomName)
    {
        var statistics = await _liveKitService.GetRoomStatisticsAsync(roomName);
        return Ok(statistics);
    }

    /// <summary>
    /// Processes LiveKit webhooks.
    /// </summary>
    [HttpPost("webhook")]
    public async Task<IActionResult> ProcessWebhook()
    {
        try
        {
            var payload = await new StreamReader(Request.Body).ReadToEndAsync();
            var authHeader = Request.Headers["Authorization"].FirstOrDefault() ?? string.Empty;

            var webhookEvent = _liveKitService.ProcessWebhook(payload, authHeader);
            await _liveKitService.HandleRoomEventAsync(webhookEvent);

            return Ok();
        }
        catch (Exception ex)
        {
            return BadRequest($"Webhook processing failed: {ex.Message}");
        }
    }
}

// DTOs for API requests
public class CreateRoomRequest
{
    [Required]
    public string RoomName { get; set; } = string.Empty;
    public int? EmptyTimeoutMinutes { get; set; }
    public int? MaxParticipants { get; set; }
    public string? Metadata { get; set; }
}

public class BroadcastMessageRequest
{
    [Required]
    public object Message { get; set; } = new();
} 