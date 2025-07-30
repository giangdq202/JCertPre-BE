using JCertPreApplication.API.Common;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Features.Users;
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
        /// Retrieves all users with pagination and filtering.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserQueryParameters parameters)
        {
            var result = await _userService.GetAllUsersAsync(parameters);
            return Ok(result);
        }

        /// <summary>
        /// Retrieves a specific user by ID.
        /// </summary>
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
        [HttpPut("{userId:guid}")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromForm] UpdateUserDto updateUserDto)
        {
            var updatedUser = await _userService.UpdateUserAsync(userId, updateUserDto);
            return Ok(updatedUser);
        }

        /// <summary>
        /// Deactivates a user account.
        /// </summary>
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
        /// Updates user avatar.
        /// </summary>
        [HttpPut("{userId:guid}/avatar")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> UpdateUserAvatar(Guid userId, IFormFile avatarFile)
        {
            var updateDto = new UpdateUserDto { AvatarFile = avatarFile };
            var updatedUser = await _userService.UpdateUserAsync(userId, updateDto);
            return Ok(updatedUser);
        }

        /// <summary>
        /// Checks if a user exists.
        /// </summary>
        [HttpHead("{userId:guid}")]
        public async Task<IActionResult> UserExists(Guid userId)
        {
            var exists = await _userService.UserExistsAsync(userId);
            return exists ? Ok() : NotFound();
        }
    }
} 