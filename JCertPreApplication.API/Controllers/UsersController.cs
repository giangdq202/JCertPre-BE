using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Features.Users;
using JCertPreApplication.API.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace JCertPreApplication.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        /// <summary>
        /// Get all users with pagination and filtering
        /// </summary>
        /// <param name="parameters">Query parameters for pagination and filtering</param>
        /// <returns>Paginated list of users</returns>
        [HttpGet]
        public async Task<IActionResult> GetAllUsers([FromQuery] UserQueryParameters parameters)
        {
            try
            {
                var result = await _userService.GetAllUsersAsync(parameters);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while retrieving users",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Get user by ID
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>User details</returns>
        [HttpGet("{userId:guid}")]
        public async Task<IActionResult> GetUserById(Guid userId)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while retrieving the user",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Update user information
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <param name="updateUserDto">Updated user information</param>
        /// <returns>Updated user details</returns>
        [HttpPut("{userId:guid}")]
        public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserDto updateUserDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var updatedUser = await _userService.UpdateUserAsync(userId, updateUserDto);
                return Ok(updatedUser);
            }
            catch (ApplicationException ex) when (ex.Message.Contains("not found"))
            {
                return NotFound(new ApiErrorResponse
                {
                    Message = ex.Message
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while updating the user",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Delete user (soft delete)
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{userId:guid}")]
        public async Task<IActionResult> DeleteUser(Guid userId)
        {
            try
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
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while deleting the user",
                    Details = ex.Message
                });
            }
        }

        /// <summary>
        /// Check if user exists
        /// </summary>
        /// <param name="userId">User ID</param>
        /// <returns>Boolean indicating if user exists</returns>
        [HttpHead("{userId:guid}")]
        public async Task<IActionResult> UserExists(Guid userId)
        {
            try
            {
                var exists = await _userService.UserExistsAsync(userId);
                return exists ? Ok() : NotFound();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiErrorResponse
                {
                    Message = "An error occurred while checking user existence",
                    Details = ex.Message
                });
            }
        }
    }
} 