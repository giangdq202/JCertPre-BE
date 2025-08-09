using AutoMapper;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Http;

namespace JCertPreApplication.Application.Features.Users
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPasswordService _passwordService;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;

        public UserService(
            IUserRepository userRepository, 
            IRoleRepository roleRepository,
            IPasswordService passwordService,
            IMapper mapper, 
            IFileService fileService)
        {
            _userRepository = userRepository;
            _roleRepository = roleRepository;
            _passwordService = passwordService;
            _mapper = mapper;
            _fileService = fileService;
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

        public async Task<AppUserDto> CreateUserAsync(CreateUserDto createUserDto)
        {
            // Check if email already exists
            var existingUser = await _userRepository.GetByEmailAsync(createUserDto.Email);
            if (existingUser != null)
            {
                throw ApiException.BadRequest("EMAIL_EXISTS", "A user with this email address already exists.");
            }

            // Get role by name
            var role = await _roleRepository.GetByRoleNameAsync(createUserDto.RoleName);
            if (role == null)
            {
                throw ApiException.BadRequest("INVALID_ROLE", $"Role '{createUserDto.RoleName}' does not exist.");
            }

            // Generate user ID
            var userId = Guid.NewGuid();
            string? avatarUrl = null;

            // Handle avatar file upload if provided
            if (createUserDto.AvatarFile != null)
            {
                // Create a custom FormFile with userId as filename
                var customFormFile = CreateCustomFormFile(createUserDto.AvatarFile, userId.ToString());

                // Upload avatar to file service
                var uploadResult = await _fileService.UploadImageAsync(customFormFile);
                
                if (uploadResult.Success && !string.IsNullOrEmpty(uploadResult.Url))
                {
                    avatarUrl = uploadResult.SecureUrl ?? uploadResult.Url;
                }
                else
                {
                    throw new InvalidOperationException($"Failed to upload avatar: {uploadResult.ErrorMessage}");
                }
            }

            // Hash password
            var hashedPassword = _passwordService.HashPassword(createUserDto.Password);

            // Create user entity
            var user = new User
            {
                userId = userId,
                fullName = createUserDto.FullName,
                email = createUserDto.Email,
                passwordHash = hashedPassword,
                phone = createUserDto.Phone,
                avatarUrl = avatarUrl,
                credit = createUserDto.Credit,
                createdAt = DateTime.UtcNow,
                lastLogin = DateTime.UtcNow,
                status = createUserDto.Status,
                roleId = role.roleId
            };

            // Save user to database
            await _userRepository.InsertAsync(user);
            await _userRepository.SaveChangesAsync();

            // Return user with role information
            var createdUser = await _userRepository.GetFirstOrDefaultAsync(
                u => u.userId == userId,
                includeProperties: "Role"
            );

            return _mapper.Map<AppUserDto>(createdUser);
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
                    if (IsCloudinaryUrl(user.avatarUrl))
                    {
                        try
                        {
                            // Extract public ID from existing avatar URL
                            var oldPublicId = ExtractCloudinaryPublicId(user.avatarUrl);
                            if (!string.IsNullOrWhiteSpace(oldPublicId))
                            {
                                await _fileService.DeleteFileAsync(oldPublicId);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Log warning but don't fail the update if old image deletion fails
                            // This could happen if the image was already deleted or doesn't exist
                            System.Diagnostics.Debug.WriteLine($"Warning: Failed to delete old avatar: {ex.Message}");
                        }
                    }
                    // Clear the old URL regardless of source
                    user.avatarUrl = null;
                }

                // Create a custom FormFile with userId as filename
                var customFormFile = CreateCustomFormFile(updateUserDto.AvatarFile, userId.ToString());

                // Upload new avatar
                var uploadResult = await _fileService.UploadImageAsync(customFormFile);
                
                if (uploadResult.Success)
                {
                    user.avatarUrl = uploadResult.SecureUrl ?? uploadResult.Url;
                }
                else
                {
                    throw new InvalidOperationException($"Failed to upload avatar: {uploadResult.ErrorMessage}");
                }
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

        public async Task<List<RoleDto>> GetAvailableRolesAsync()
        {
            var roles = await _roleRepository.GetAllAsync();
            return roles.Select(r => new RoleDto
            {
                RoleId = r.roleId,
                RoleName = r.roleName,
                Description = r.description
            }).ToList();
        }

        #region Private Helper Methods

        private static bool IsCloudinaryUrl(string? url)
        {
            return !string.IsNullOrWhiteSpace(url)
                   && url.Contains("res.cloudinary.com", StringComparison.OrdinalIgnoreCase);
        }

        private static string? ExtractCloudinaryPublicId(string? url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;

            try
            {
                var uri = new Uri(url);
                var segments = uri.AbsolutePath.Split('/', StringSplitOptions.RemoveEmptyEntries);
                var uploadIdx = Array.IndexOf(segments, "upload");

                if (uploadIdx == -1 || uploadIdx >= segments.Length - 1)
                    return null;

                // Get everything after upload/ as potential public ID
                var publicIdParts = segments.Skip(uploadIdx + 1).ToArray();

                // Skip version if present (starts with 'v' followed by numbers)
                if (publicIdParts.Length > 0 && publicIdParts[0].StartsWith("v") &&
                    publicIdParts[0].Length > 1 && publicIdParts[0].Skip(1).All(char.IsDigit))
                {
                    publicIdParts = publicIdParts.Skip(1).ToArray();
                }

                if (publicIdParts.Length == 0) return null;

                // Remove file extension from the last part
                var lastPart = publicIdParts.Last();
                var dotIndex = lastPart.LastIndexOf('.');
                if (dotIndex > 0)
                {
                    publicIdParts[publicIdParts.Length - 1] = lastPart.Substring(0, dotIndex);
                }
                
                return string.Join("/", publicIdParts);
            }
            catch
            {
                return null;
            }
        }

        private static IFormFile CreateCustomFormFile(IFormFile originalFile, string customFileName)
        {
            // Get the file extension from original file
            var extension = Path.GetExtension(originalFile.FileName);
            var newFileName = customFileName + extension;

            return new CustomFormFile(originalFile, newFileName);
        }

        private static string? ExtractPublicIdFromUrl(string cloudinaryUrl)
        {
            // Keep existing method for backward compatibility
            return ExtractCloudinaryPublicId(cloudinaryUrl);
        }

// Removed unused methods IsImageFile and IsVideoFile
        #endregion
    }

    /// <summary>
    /// Custom IFormFile implementation to override filename while preserving original file content
    /// </summary>
    internal class CustomFormFile : IFormFile
    {
        private readonly IFormFile _originalFile;
        private readonly string _customFileName;

        public CustomFormFile(IFormFile originalFile, string customFileName)
        {
            _originalFile = originalFile;
            _customFileName = customFileName;
        }

        public string ContentType => _originalFile.ContentType;
        public string ContentDisposition => _originalFile.ContentDisposition;
        public IHeaderDictionary Headers => _originalFile.Headers;
        public long Length => _originalFile.Length;
        public string Name => _originalFile.Name;
        public string FileName => _customFileName; // This is the overridden filename

        public void CopyTo(Stream target) => _originalFile.CopyTo(target);
        public Task CopyToAsync(Stream target, CancellationToken cancellationToken = default) =>
            _originalFile.CopyToAsync(target, cancellationToken);
        public Stream OpenReadStream() => _originalFile.OpenReadStream();
    }
}

