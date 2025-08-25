using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.User;
using JCertPreApplication.Application.Features.Users;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using Moq;
using AutoMapper;
using JCertPreApplication.Application.Utilities;
using Xunit;
using JCertPreApplication.Application.Exceptions;

namespace JCertPreApplication.UnitTests.Features.Users;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IRoleRepository> _mockRoleRepository;
    private readonly Mock<IPasswordService> _mockPasswordService;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IFileService> _mockFileService;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockRoleRepository = new Mock<IRoleRepository>();
        _mockPasswordService = new Mock<IPasswordService>();
        _mockMapper = new Mock<IMapper>();
        _mockFileService = new Mock<IFileService>();

        _userService = new UserService(
            _mockUserRepository.Object,
            _mockRoleRepository.Object,
            _mockPasswordService.Object,
            _mockMapper.Object,
            _mockFileService.Object);
    }

    #region GetAllUsersAsync Tests

    [Fact]
    public async Task GetAllUsersAsync_WithValidParams_ShouldReturnPaginatedUsers()
    {
        // Arrange
        var parameters = new UserQueryParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var users = new List<Domain.Entities.User>
        {
            new UserBuilder().WithId(Guid.NewGuid()).WithName("Test User 1").WithEmail("test1@example.com").Build(),
            new UserBuilder().WithId(Guid.NewGuid()).WithName("Test User 2").WithEmail("test2@example.com").Build(),
            new UserBuilder().WithId(Guid.NewGuid()).WithName("Test User 3").WithEmail("test3@example.com").Build()
        };

        var paginatedUsers = new Pagination<Domain.Entities.User>
        {
            Items = users,
            TotalItemsCount = 3,
            PageIndex = 1,
            PageSize = 10
        };

        var userDtos = users.Select(u => new AppUserDto
        {
            Id = u.userId,
            fullName = u.fullName,
            email = u.email
        }).ToList();

        _mockUserRepository.Setup(r => r.GetPaginationAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.User, bool>>>(),
                "Role",
                1,
                10,
                It.IsAny<Func<IQueryable<Domain.Entities.User>, IOrderedQueryable<Domain.Entities.User>>>()))
            .ReturnsAsync(paginatedUsers);

        _mockMapper.Setup(m => m.Map<List<AppUserDto>>(It.IsAny<List<Domain.Entities.User>>()))
            .Returns(userDtos);

        // Act
        var result = await _userService.GetAllUsersAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(3);
        result.TotalItemsCount.Should().Be(3);
        result.PageIndex.Should().Be(1);
        result.PageSize.Should().Be(10);

        _mockUserRepository.Verify(r => r.GetPaginationAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.User, bool>>>(),
            "Role",
            1,
            10,
            It.IsAny<Func<IQueryable<Domain.Entities.User>, IOrderedQueryable<Domain.Entities.User>>>()), Times.Once);
    }

    [Fact]
    public async Task GetAllUsersAsync_WithNoResults_ShouldReturnEmptyPagination()
    {
        // Arrange
        var parameters = new UserQueryParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var emptyPaginatedUsers = new Pagination<Domain.Entities.User>
        {
            Items = new List<Domain.Entities.User>(),
            TotalItemsCount = 0,
            PageIndex = 1,
            PageSize = 10
        };

        _mockUserRepository.Setup(r => r.GetPaginationAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.User, bool>>>(),
                "Role",
                1,
                10,
                It.IsAny<Func<IQueryable<Domain.Entities.User>, IOrderedQueryable<Domain.Entities.User>>>()))
            .ReturnsAsync(emptyPaginatedUsers);

        _mockMapper.Setup(m => m.Map<List<AppUserDto>>(It.IsAny<List<Domain.Entities.User>>()))
            .Returns(new List<AppUserDto>());

        // Act
        var result = await _userService.GetAllUsersAsync(parameters);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalItemsCount.Should().Be(0);
        result.PageIndex.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    #endregion

    #region GetUserByIdAsync Tests

    [Fact]
    public async Task GetUserByIdAsync_WithExistingId_ShouldReturnUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserBuilder()
            .WithId(userId)
            .WithName("Test User")
            .WithEmail("test@example.com")
            .Build();

        var userDto = new AppUserDto
        {
            Id = userId,
            fullName = "Test User",
            email = "test@example.com"
        };

        _mockUserRepository.Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.User, bool>>>(),
                "Role"))
            .ReturnsAsync(user);

        _mockMapper.Setup(m => m.Map<AppUserDto>(user))
            .Returns(userDto);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(userId);
        result.fullName.Should().Be("Test User");
        result.email.Should().Be("test@example.com");

        _mockUserRepository.Verify(r => r.GetFirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.User, bool>>>(),
            "Role"), Times.Once);
    }

    [Fact]
    public async Task GetUserByIdAsync_WithNonExistentId_ShouldReturnNull()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserRepository.Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.User, bool>>>(),
                "Role"))
            .ReturnsAsync((Domain.Entities.User?)null);

        // Act
        var result = await _userService.GetUserByIdAsync(userId);

        // Assert
        result.Should().BeNull();

        _mockUserRepository.Verify(r => r.GetFirstOrDefaultAsync(
            It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.User, bool>>>(),
            "Role"), Times.Once);
    }

    #endregion

    #region CreateUserAsync Tests

    [Fact]
    public async Task CreateUserAsync_WithValidData_ShouldCreateAndReturnUser()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            FullName = "New User",
            Email = "newuser@example.com",
            Password = "password123",
            Phone = "1234567890",
            RoleName = "STUDENT"
        };

        var role = new Role
        {
            roleId = Guid.NewGuid(),
            roleName = "STUDENT",
            description = "Student role"
        };

        var createdUser = new UserBuilder()
            .WithId(Guid.NewGuid())
            .WithName("New User")
            .WithEmail("newuser@example.com")
            .WithRole(role)
            .Build();
        createdUser.phone = "1234567890";

        var userDto = new AppUserDto
        {
            Id = createdUser.userId,
            fullName = "New User",
            email = "newuser@example.com"
        };

        _mockUserRepository.Setup(r => r.GetByEmailAsync("newuser@example.com"))
            .ReturnsAsync((Domain.Entities.User?)null);

        _mockRoleRepository.Setup(r => r.GetByRoleNameAsync("STUDENT"))
            .ReturnsAsync(role);

        _mockPasswordService.Setup(p => p.HashPassword("password123"))
            .Returns("hashedPassword");

        _mockUserRepository.Setup(r => r.InsertAsync(It.IsAny<Domain.Entities.User>()))
            .ReturnsAsync(createdUser);

        _mockUserRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockUserRepository.Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.User, bool>>>(),
                "Role"))
            .ReturnsAsync(createdUser);

        _mockMapper.Setup(m => m.Map<AppUserDto>(createdUser))
            .Returns(userDto);

        // Act
        var result = await _userService.CreateUserAsync(createUserDto);

        // Assert
        result.Should().NotBeNull();
        result!.fullName.Should().Be("New User");
        result.email.Should().Be("newuser@example.com");

        _mockUserRepository.Verify(r => r.InsertAsync(It.IsAny<Domain.Entities.User>()), Times.Once);
        _mockPasswordService.Verify(p => p.HashPassword("password123"), Times.Once);
        _mockRoleRepository.Verify(r => r.GetByRoleNameAsync("STUDENT"), Times.Once);
    }

    [Fact]
    public async Task CreateUserAsync_WithExistingEmail_ShouldThrowBadRequestException()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            FullName = "New User",
            Email = "existing@example.com",
            Password = "password123",
            Phone = "1234567890",
            RoleName = "STUDENT"
        };

        var existingUser = new UserBuilder()
            .WithEmail("existing@example.com")
            .Build();

        _mockUserRepository.Setup(r => r.GetByEmailAsync("existing@example.com"))
            .ReturnsAsync(existingUser);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _userService.CreateUserAsync(createUserDto));
        
        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("EMAIL_EXISTS");

        _mockUserRepository.Verify(r => r.InsertAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    [Fact]
    public async Task CreateUserAsync_WithInvalidRole_ShouldThrowBadRequestException()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            FullName = "New User",
            Email = "newuser@example.com",
            Password = "password123",
            Phone = "1234567890",
            RoleName = "INVALID_ROLE"
        };

        _mockUserRepository.Setup(r => r.GetByEmailAsync("newuser@example.com"))
            .ReturnsAsync((Domain.Entities.User?)null);

        _mockRoleRepository.Setup(r => r.GetByRoleNameAsync("INVALID_ROLE"))
            .ReturnsAsync((Role?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _userService.CreateUserAsync(createUserDto));
        
        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        exception.ErrorCode.Should().Be("INVALID_ROLE");

        _mockUserRepository.Verify(r => r.InsertAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    #endregion

    #region UpdateUserAsync Tests

    [Fact]
    public async Task UpdateUserAsync_WithValidData_ShouldUpdateAndReturnUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateUserDto = new UpdateUserDto
        {
            FullName = "Updated Name",
            Phone = "9876543210"
        };

        var existingUser = new UserBuilder()
            .WithId(userId)
            .WithName("Original Name")
            .WithEmail("user@example.com")
            .Build();

        var updatedUser = new UserBuilder()
            .WithId(userId)
            .WithName("Updated Name")
            .WithEmail("user@example.com")
            .Build();
        updatedUser.phone = "9876543210";

        var userDto = new AppUserDto
        {
            Id = userId,
            fullName = "Updated Name",
            email = "user@example.com",
            phone = "9876543210"
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.User>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        _mockUserRepository.Setup(r => r.GetFirstOrDefaultAsync(
                It.IsAny<System.Linq.Expressions.Expression<Func<Domain.Entities.User, bool>>>(),
                "Role"))
            .ReturnsAsync(updatedUser);

        _mockMapper.Setup(m => m.Map<AppUserDto>(updatedUser))
            .Returns(userDto);

        // Act
        var result = await _userService.UpdateUserAsync(userId, updateUserDto);

        // Assert
        result.Should().NotBeNull();
        result!.fullName.Should().Be("Updated Name");
        result.phone.Should().Be("9876543210");

        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.User>()), Times.Once);
    }

    [Fact]
    public async Task UpdateUserAsync_WithNonExistentId_ShouldThrowNotFoundException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateUserDto = new UpdateUserDto
        {
            FullName = "Updated Name"
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((Domain.Entities.User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(() => _userService.UpdateUserAsync(userId, updateUserDto));
        
        exception.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);

        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    #endregion

    #region DeleteUserAsync Tests

    [Fact]
    public async Task DeleteUserAsync_WithExistingId_ShouldDeleteUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserBuilder()
            .WithId(userId)
            .Build();

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<Domain.Entities.User>()))
            .Returns(Task.CompletedTask);

        _mockUserRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        result.Should().BeTrue();

        _mockUserRepository.Verify(r => r.UpdateAsync(It.Is<Domain.Entities.User>(u => u.status == UserStatus.Inactive)), Times.Once);
    }

    [Fact]
    public async Task DeleteUserAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((Domain.Entities.User?)null);

        // Act
        var result = await _userService.DeleteUserAsync(userId);

        // Assert
        result.Should().BeFalse();

        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.User>()), Times.Never);
    }

    #endregion

    #region UserExistsAsync Tests

    [Fact]
    public async Task UserExistsAsync_WithExistingId_ShouldReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new UserBuilder().WithId(userId).Build();

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.UserExistsAsync(userId);

        // Assert
        result.Should().BeTrue();

        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task UserExistsAsync_WithNonExistentId_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync((Domain.Entities.User?)null);

        // Act
        var result = await _userService.UserExistsAsync(userId);

        // Assert
        result.Should().BeFalse();

        _mockUserRepository.Verify(r => r.GetByIdAsync(userId), Times.Once);
    }

    #endregion

    #region GetAvailableRolesAsync Tests

    [Fact]
    public async Task GetAvailableRolesAsync_ShouldReturnAllRoles()
    {
        // Arrange
        var roles = new List<Role>
        {
            new Role { roleId = Guid.NewGuid(), roleName = "ADMIN", description = "Admin role" },
            new Role { roleId = Guid.NewGuid(), roleName = "STUDENT", description = "Student role" },
            new Role { roleId = Guid.NewGuid(), roleName = "INSTRUCTOR", description = "Instructor role" }
        };

        var roleDtos = roles.Select(r => new RoleDto
        {
            RoleId = r.roleId,
            RoleName = r.roleName,
            Description = r.description
        }).ToList();

        _mockRoleRepository.Setup(r => r.GetAllAsync(It.IsAny<string>()))
            .ReturnsAsync(roles);

        // Act
        var result = await _userService.GetAvailableRolesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(3);
        result.Should().Contain(r => r.RoleName == "ADMIN");
        result.Should().Contain(r => r.RoleName == "STUDENT");
        result.Should().Contain(r => r.RoleName == "INSTRUCTOR");

        _mockRoleRepository.Verify(r => r.GetAllAsync(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetAvailableRolesAsync_WithNoRoles_ShouldReturnEmptyList()
    {
        // Arrange
        var roles = new List<Role>();

        _mockRoleRepository.Setup(r => r.GetAllAsync(It.IsAny<string>()))
            .ReturnsAsync(roles);

        // Act
        var result = await _userService.GetAvailableRolesAsync();

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockRoleRepository.Verify(r => r.GetAllAsync(It.IsAny<string>()), Times.Once);
    }

    #endregion
}
