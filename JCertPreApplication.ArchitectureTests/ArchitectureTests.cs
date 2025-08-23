using FluentAssertions;
using NetArchTest.Rules;
using System.Reflection;

namespace JCertPreApplication.ArchitectureTests;

public class ArchitectureTests
{
    private static readonly Assembly DomainAssembly = typeof(JCertPreApplication.Domain.Entities.User).Assembly;
    private static readonly Assembly ApplicationAssembly = typeof(JCertPreApplication.Application.Features.Auth.AuthService).Assembly;
    private static readonly Assembly PersistenceAssembly = typeof(JCertPreApplication.Persistence.Repositories.UserRepository).Assembly;
    private static readonly Assembly ApiAssembly = typeof(JCertPreApplication.API.Controllers.AuthController).Assembly;

    [Fact]
    public void Domain_Should_NotHaveDependencyOnOtherProjects()
    {
        // Arrange & Act
        var result = Types.InAssembly(DomainAssembly)
            .Should()
            .NotHaveDependencyOnAll(
                "JCertPreApplication.Application",
                "JCertPreApplication.Persistence",
                "JCertPreApplication.API");

        // Assert
        result.GetResult().IsSuccessful.Should().BeTrue("Domain layer should not depend on any other project layers");
    }

    [Fact]
    public void Application_Should_NotHaveDependencyOnPersistence()
    {
        // Arrange & Act
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOnAll(
                "JCertPreApplication.Persistence",
                "JCertPreApplication.API");

        // Assert
        result.Should().BeTrue("Application layer should not depend on Persistence or API layers");
    }

    [Fact]
    public void Application_Should_OnlyDependOnDomain()
    {
        // Arrange & Act
        var result = Types.InAssembly(ApplicationAssembly)
            .Should()
            .NotHaveDependencyOnAll(
                "JCertPreApplication.Persistence",
                "JCertPreApplication.API");

        // Assert
        result.Should().BeTrue("Application layer should only depend on Domain layer");
    }

    [Fact]
    public void Controllers_Should_NotHaveDependencyOnPersistence()
    {
        // Arrange & Act
        var result = Types.InAssembly(ApiAssembly)
            .That().ResideInNamespace("JCertPreApplication.API.Controllers")
            .Should()
            .NotHaveDependencyOn("JCertPreApplication.Persistence");

        // Assert
        result.Should().BeTrue("Controllers should not directly depend on Persistence layer");
    }

    [Fact]
    public void Services_Should_BeInCorrectNamespace()
    {
        // Arrange & Act
        var result = Types.InAssembly(ApplicationAssembly)
            .That().HaveNameEndingWith("Service")
            .Should()
            .ResideInNamespaceStartingWith("JCertPreApplication.Application.Features");

        // Assert
        result.Should().BeTrue("Services should be in Features namespace");
    }

    [Fact]
    public void Services_Should_ImplementInterfaces()
    {
        // Arrange & Act
        var result = Types.InAssembly(ApplicationAssembly)
            .That().HaveNameEndingWith("Service")
            .And().AreClasses()
            .Should()
            .BeInterfaces();

        // Assert
        result.Should().BeTrue("All services should implement their corresponding interfaces");
    }

    [Fact]
    public void Repositories_Should_BeInCorrectNamespace()
    {
        // Arrange & Act
        var result = Types.InAssembly(PersistenceAssembly)
            .That().HaveNameEndingWith("Repository")
            .Should()
            .ResideInNamespace("JCertPreApplication.Persistence.Repositories");

        // Assert
        result.Should().BeTrue("Repositories should be in Repositories namespace");
    }

    [Fact]
    public void Repositories_Should_ImplementInterfaces()
    {
        // Arrange & Act
        var result = Types.InAssembly(PersistenceAssembly)
            .That().HaveNameEndingWith("Repository")
            .And().AreClasses()
            .Should()
            .BeInterfaces();

        // Assert
        result.Should().BeTrue("All repositories should implement their corresponding interfaces");
    }

    [Fact]
    public void Controllers_Should_BeInCorrectNamespace()
    {
        // Arrange & Act
        var result = Types.InAssembly(ApiAssembly)
            .That().HaveNameEndingWith("Controller")
            .Should()
            .ResideInNamespace("JCertPreApplication.API.Controllers");

        // Assert
        result.Should().BeTrue("Controllers should be in Controllers namespace");
    }

    [Fact]
    public void Controllers_Should_InheritFromControllerBase()
    {
        // Arrange & Act
        var result = Types.InAssembly(ApiAssembly)
            .That().HaveNameEndingWith("Controller")
            .And().AreClasses()
            .Should()
            .Inherit(typeof(Microsoft.AspNetCore.Mvc.ControllerBase));

        // Assert
        result.Should().BeTrue("All controllers should inherit from ControllerBase");
    }

    [Fact]
    public void Entities_Should_BeInDomainEntitiesNamespace()
    {
        // Arrange & Act
        var result = Types.InAssembly(DomainAssembly)
            .That().AreClasses()
            .And().ResideInNamespace("JCertPreApplication.Domain.Entities")
            .Should()
            .NotBeAbstract();

        // Assert
        result.Should().BeTrue("Domain entities should be concrete classes");
    }

    [Fact]
    public void Dtos_Should_BeInCorrectNamespace()
    {
        // Arrange & Act
        var result = Types.InAssembly(ApplicationAssembly)
            .That().HaveNameEndingWith("Dto")
            .Should()
            .ResideInNamespaceStartingWith("JCertPreApplication.Application.Dtos");

        // Assert
        result.Should().BeTrue("DTOs should be in Dtos namespace");
    }
}
