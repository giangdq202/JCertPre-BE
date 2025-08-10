using System.Reflection;
using JCertPreApplication.Domain.Configuration;

namespace JCertPreApplication.API.Extensions;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Automatically binds, validates, and registers a configuration class with the DI container
    /// </summary>
    /// <typeparam name="T">The configuration class type</typeparam>
    /// <param name="services">The service collection</param>
    /// <param name="configuration">The configuration instance</param>
    /// <param name="isValidationOptional">Whether validation failures should be treated as warnings instead of errors</param>
    /// <param name="registerAsSingleton">Whether to register the configuration instance as a singleton in addition to IOptions</param>
    /// <returns>The configured instance for chaining</returns>
    public static T AddValidatedConfiguration<T>(
        this IServiceCollection services, 
        IConfiguration configuration, 
        bool isValidationOptional = false,
        bool registerAsSingleton = false) where T : class, new()
    {
        // Get the section name from the configuration class
        var sectionName = GetSectionName<T>();
        
        // Create and bind the configuration instance
        var configInstance = new T();
        configuration.GetSection(sectionName).Bind(configInstance);
        
        // Validate the configuration if it has a Validate method
        ValidateConfiguration(configInstance, isValidationOptional);
        
        // Register with DI container using IOptions pattern
        services.Configure<T>(configuration.GetSection(sectionName));
        
        // Optionally register as singleton (useful for direct injection)
        if (registerAsSingleton)
        {
            services.AddSingleton(configInstance);
        }
        
        return configInstance;
    }
    
    private static string GetSectionName<T>()
    {
        // Try to get SectionName from a public static field or property
        var sectionNameField = typeof(T).GetField("SectionName", BindingFlags.Public | BindingFlags.Static);
        if (sectionNameField?.GetValue(null) is string fieldValue)
        {
            return fieldValue;
        }
        
        var sectionNameProperty = typeof(T).GetProperty("SectionName", BindingFlags.Public | BindingFlags.Static);
        if (sectionNameProperty?.GetValue(null) is string propertyValue)
        {
            return propertyValue;
        }
        
        // Fallback: use the class name without "Configuration" suffix
        var typeName = typeof(T).Name;
        return typeName.EndsWith("Configuration") 
            ? typeName.Substring(0, typeName.Length - "Configuration".Length)
            : typeName;
    }
    
    private static void ValidateConfiguration<T>(T configInstance, bool isOptional)
    {
        // Look for a Validate method using reflection
        var validateMethod = typeof(T).GetMethod("Validate", BindingFlags.Public | BindingFlags.Instance, null, Type.EmptyTypes, null);
        
        if (validateMethod == null) return; // No validation method found
        
        try
        {
            validateMethod.Invoke(configInstance, null);
        }
        catch (Exception ex)
        {
            var actualException = ex.InnerException ?? ex;
            var configTypeName = typeof(T).Name;
            
            if (isOptional)
            {
                Console.WriteLine($"Warning: {configTypeName} validation failed: {actualException.Message}");
            }
            else
            {
                throw new InvalidOperationException($"{configTypeName} validation failed: {actualException.Message}", actualException);
            }
        }
    }
}
