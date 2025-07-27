using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;

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
    /// <param name="role">The role of the participant (instructor or student, defaults to student).</param>
    /// <returns>A JWT token that can be used to connect to LiveKit.</returns>
    [HttpGet("token")]
    public IActionResult GetToken(
        [FromQuery] string roomName,
        [FromQuery] string? participantIdentity = null,
        [FromQuery] string? participantName = null,
        [FromQuery] string role = "student")
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
} 