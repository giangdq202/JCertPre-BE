using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;

namespace JCertPreApplication.UnitTests.Common.Builders;

public class UserBuilder
{
    private User _user;

    public UserBuilder()
    {
        _user = new User
        {
            userId = Guid.NewGuid(),
            email = "test@example.com",
            passwordHash = "hashedpassword",
            fullName = "Test User",
            status = UserStatus.Active,
            createdAt = DateTime.UtcNow,
            credit = 0
        };
    }

    public static UserBuilder Create() => new UserBuilder();

    public UserBuilder WithId(Guid id)
    {
        _user.userId = id;
        return this;
    }

    public UserBuilder WithEmail(string email)
    {
        _user.email = email;
        return this;
    }

    public UserBuilder WithName(string fullName)
    {
        _user.fullName = fullName;
        return this;
    }

    public UserBuilder WithPasswordHash(string passwordHash)
    {
        _user.passwordHash = passwordHash;
        return this;
    }

    public UserBuilder WithRole(Role role)
    {
        _user.Role = role;
        _user.roleId = role.roleId;
        return this;
    }

    public UserBuilder AsInactive()
    {
        _user.status = UserStatus.Inactive;
        return this;
    }

    public UserBuilder WithCredits(int credits)
    {
        _user.credit = credits;
        return this;
    }

    public UserBuilder WithCreatedAt(DateTime createdAt)
    {
        _user.createdAt = createdAt;
        return this;
    }

    public User Build() => _user;
}
