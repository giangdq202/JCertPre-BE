using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace JCertPreApplication.UnitTests.Common.TestFixtures;

public class DatabaseFixture : IDisposable
{
    public JCertPreDatabaseContext Context { get; private set; }

    public DatabaseFixture()
    {
        var options = new DbContextOptionsBuilder<JCertPreDatabaseContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new JCertPreDatabaseContext(options);
        Context.Database.EnsureCreated();
        SeedData();
    }

    private void SeedData()
    {
        // Seed common test data
        var adminRole = new Role 
        { 
            roleId = Guid.NewGuid(), 
            roleName = "ADMIN"
        };
        
        var studentRole = new Role 
        { 
            roleId = Guid.NewGuid(), 
            roleName = "STUDENT"
        };
        
        var instructorRole = new Role 
        { 
            roleId = Guid.NewGuid(), 
            roleName = "INSTRUCTOR"
        };

        var academicManagerRole = new Role 
        { 
            roleId = Guid.NewGuid(), 
            roleName = "ACADEMIC_MANAGER"
        };

        Context.Roles.AddRange(adminRole, studentRole, instructorRole, academicManagerRole);
        Context.SaveChanges();
    }

    public void ClearData()
    {
        Context.Users.RemoveRange(Context.Users);
        Context.Courses.RemoveRange(Context.Courses);
        Context.Enrollments.RemoveRange(Context.Enrollments);
        Context.SaveChanges();
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
