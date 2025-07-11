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

        public UserService(IUserRepository userRepository, IMapper mapper)
        {
            _userRepository = userRepository;
            _mapper = mapper;
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

            if (!string.IsNullOrWhiteSpace(updateUserDto.AvatarUrl))
            {
                user.avatarUrl = updateUserDto.AvatarUrl;
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
    }
} 