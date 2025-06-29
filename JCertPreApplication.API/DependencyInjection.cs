using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using JCertPreApplication.Domain.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;

namespace JCertPreApplication.API
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Configures API-specific services including controllers, Swagger, authentication, and CORS.
        /// </summary>
        /// <param name="services">The service collection</param>
        /// <param name="configuration">The application configuration</param>
        /// <returns>The service collection for method chaining</returns>
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Core API services
            AddCoreApiServices(services);
            
            // Swagger documentation
            AddSwaggerServices(services);
            
            // Authentication & Authorization
            AddAuthenticationServices(services, configuration);
            
            // CORS
            AddCorsServices(services, configuration);

            return services;
        }

        private static void AddCoreApiServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
        }

        private static void AddSwaggerServices(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo 
                { 
                    Title = "JCertPre API", 
                    Description = "API for JCertPre Application - Learning and Certification Platform",
                    Contact = new OpenApiContact
                    {
                        Name = "JCertPre Support",
                        Email = "support@jcertpre.com"
                    }
                });
                
                // Add JWT Authentication to Swagger
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: 'Bearer {token}'",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer"
                });
                
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            },
                            Scheme = "oauth2",
                            Name = "Bearer",
                            In = ParameterLocation.Header,
                        },
                        new List<string>()
                    }
                });
                
                // Group controllers by feature/domain
                c.TagActionsBy(api =>
                {
                    var controllerName = api.ActionDescriptor.RouteValues["controller"];
                    return new[] { controllerName };
                });
                
                // Enable XML comments if available
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath);
                }
            });
        }

        private static void AddAuthenticationServices(IServiceCollection services, IConfiguration configuration)
        {
            var jwtConfig = new JwtConfiguration();
            configuration.GetSection(JwtConfiguration.SectionName).Bind(jwtConfig);
            
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfig.Issuer,
                        ValidAudience = jwtConfig.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey)),
                        ClockSkew = TimeSpan.Zero
                    };
                });
        }

        private static void AddCorsServices(IServiceCollection services, IConfiguration configuration)
        {
            var corsConfig = new CorsConfiguration();
            configuration.GetSection(CorsConfiguration.SectionName).Bind(corsConfig);
            
            services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigins", policy =>
                {
                    policy.WithOrigins(corsConfig.GetAllowedOriginsArray())
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });
        }


    }
}
