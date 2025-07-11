using JCertPreApplication.API.Common;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Features.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    /// <summary>
    /// Handles user management operations including retrieval, update, and deletion.
    /// </summary>
    [Route("api/users")]
    [ApiController]
    [Tags("User Management")]
    [Produces("application/json")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        /// <summary>
        /// Retrieves all users with pagination and filtering capabilities.
        /// </summary>
        /// <param name="parameters">Query parameters for pagination and filtering.</param>
        /// <returns>Paginated list of users with their basic information.</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserQueryParameters parameters)
        {
            var result = await _userService.GetAllUsersAsync(parameters);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific user by their unique identifier.
        /// </summary>
        /// <param name="userId">The unique identifier of the user.</param>
        /// <returns>User details including profile information.</returns>
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            var user = await _userService.GetUserByIdAsync(userId);
            if (user == null)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"User with ID {userId} not found"
                });
            }

            return Ok(user);
        }

        /// <summary>
        /// Updates user profile information.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to update.</param>
        /// <param name="updateUserDto">The updated user information.</param>
        /// <returns>Updated user profile with current information.</returns>
        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto updateUserDto)
        {
            var updatedUser = await _userService.UpdateUserAsync(userId, updateUserDto);
            return Ok(updatedUser);
        }

        /// <summary>
        /// Deactivates a user account (soft delete).
        /// </summary>
        /// <param name="userId">The unique identifier of the user to deactivate.</param>
        /// <returns>Confirmation of successful deactivation.</returns>
        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            var result = await _userService.DeleteUserAsync(userId);
            if (!result)
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = $"User with ID {userId} not found"
                });
            }

            return NoContent();
        }

        /// <summary>
        /// Verifies if a user exists in the system.
        /// </summary>
        /// <param name="userId">The unique identifier of the user to check.</param>
        /// <returns>HTTP 200 if user exists, HTTP 404 if not found.</returns>
        [HttpHead("{userId:guid}")]
        public async Task<IActionResult> UserExists(Guid userId)
        {
            var exists = await _userService.UserExistsAsync(userId);
            return exists ? Ok() : NotFound();
        }
    }
} 