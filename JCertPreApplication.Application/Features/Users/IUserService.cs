using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Utilities;

namespace JCertPreApplication.Application.Features.Users
{
    public interface IUserService
    {
        Task<Pagination<AppUserDto>> GetAllUsersAsync(UserQueryParameters parameters);
        Task<AppUserDto?> GetUserByIdAsync(Guid userId);
        Task<AppUserDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto);
        Task<bool> DeleteUserAsync(Guid userId);
        Task<bool> UserExistsAsync(Guid userId);
    }
} 