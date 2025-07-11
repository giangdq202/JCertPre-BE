using AutoMapper;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using System.Linq.Expressions;

namespace JCertPreApplication.Application.Features.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;
        private readonly ICloudinaryService _cloudinaryService;

        public UserService(IUserRepository userRepository, IMapper mapper, ICloudinaryService cloudinaryService)
        {
            _userRepository = userRepository;
            _mapper = mapper;
            _cloudinaryService = cloudinaryService;
        }

        public async Task<Pagination<AppUserDto>> GetAllUsersAsync(UserQueryParameters parameters)
        {
            Expression<Func<User, bool>>? predicate = null;

            // Build filter predicate
            var predicates = new List<Expression<Func<User, bool>>>();

            if (!string.IsNullOrWhiteSpace(parameters.SearchQuery))
            {
                predicates.Add(u => u.fullName.ToLower().Contains(parameters.SearchQuery.ToLower()) ||
                                   u.email.ToLower().Contains(parameters.SearchQuery.ToLower()));
            }

            if (parameters.Status.HasValue)
            {
                predicates.Add(u => u.status == parameters.Status.Value);
            }

            if (parameters.RoleId.HasValue)
            {
                predicates.Add(u => u.roleId == parameters.RoleId.Value);
            }

            // Combine predicates using AND logic
            if (predicates.Any())
            {
                predicate = predicates.Aggregate((prev, next) => prev.And(next));
            }

            var paginatedUsers = await _userRepository.GetPaginationAsync(
                predicate: predicate,
                includeProperties: "Role",
                pageIndex: parameters.PageNumber,
                pageSize: parameters.PageSize
            );

            var userDtos = _mapper.Map<List<AppUserDto>>(paginatedUsers.Items);

            return new Pagination<AppUserDto>
            {
                Items = userDtos,
                TotalItemsCount = paginatedUsers.TotalItemsCount,
                PageIndex = paginatedUsers.PageIndex,
                PageSize = paginatedUsers.PageSize
            };
        }

        public async Task<AppUserDto?> GetUserByIdAsync(Guid userId)
        {
            var user = await _userRepository.GetFirstOrDefaultAsync(
                u => u.userId == userId,
                includeProperties: "Role"
            );

            return user != null ? _mapper.Map<AppUserDto>(user) : null;
        }

        public async Task<AppUserDto> UpdateUserAsync(Guid userId, UpdateUserDto updateUserDto)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw ApiException.NotFound("User", userId);
            }

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(updateUserDto.FullName))
            {
                user.fullName = updateUserDto.FullName;
            }

            if (!string.IsNullOrWhiteSpace(updateUserDto.Phone))
            {
                user.phone = updateUserDto.Phone;
            }

            // Handle avatar file upload
            if (updateUserDto.AvatarFile != null)
            {
                // Delete old avatar if exists
                if (!string.IsNullOrWhiteSpace(user.avatarUrl))
                {
                    try
                    {
                        // Extract public ID from existing avatar URL
                        var oldPublicId = ExtractPublicIdFromUrl(user.avatarUrl);
                        if (!string.IsNullOrWhiteSpace(oldPublicId))
                        {
                            await _cloudinaryService.DeleteImageAsync(oldPublicId);
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log warning but don't fail the update if old image deletion fails
                        // This could happen if the image was already deleted or doesn't exist
                        System.Diagnostics.Debug.WriteLine($"Warning: Failed to delete old avatar: {ex.Message}");
                    }
                }

                // Upload new avatar
                var uploadResult = await _cloudinaryService.UploadImageAsync(updateUserDto.AvatarFile);
                user.avatarUrl = uploadResult.SecureUrl.ToString();
            }

            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            // Get updated user with role information
            var updatedUser = await _userRepository.GetFirstOrDefaultAsync(
                u => u.userId == userId,
                includeProperties: "Role"
            );

            return _mapper.Map<AppUserDto>(updatedUser);
        }

        public async Task<bool> DeleteUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Soft delete by changing status to Inactive
            user.status = UserStatus.Inactive;
            await _userRepository.UpdateAsync(user);
            await _userRepository.SaveChangesAsync();

            return true;
        }

        public async Task<bool> UserExistsAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            return user != null;
        }

        private static string? ExtractPublicIdFromUrl(string cloudinaryUrl)
        {
            if (string.IsNullOrWhiteSpace(cloudinaryUrl))
                return null;

            try
            {
                // Cloudinary URL format: https://res.cloudinary.com/{cloud_name}/{resource_type}/upload/{public_id}.{format}
                // Example: https://res.cloudinary.com/demo/image/upload/v1234567890/sample.jpg
                
                var uri = new Uri(cloudinaryUrl);
                var pathSegments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                
                // Find the upload segment
                var uploadIndex = Array.IndexOf(pathSegments, "upload");
                if (uploadIndex == -1 || uploadIndex >= pathSegments.Length - 1)
                    return null;

                // Get everything after upload/ as the public ID (may include folders and version)
                var publicIdParts = pathSegments.Skip(uploadIndex + 1).ToArray();
                var publicIdWithExtension = string.Join("/", publicIdParts);
                
                // Remove file extension
                var lastDotIndex = publicIdWithExtension.LastIndexOf('.');
                if (lastDotIndex > 0)
                {
                    return publicIdWithExtension.Substring(0, lastDotIndex);
                }
                
                return publicIdWithExtension;
            }
            catch
            {
                return null;
            }
        }
    }
} 