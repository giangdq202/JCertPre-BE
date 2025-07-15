# Build stage
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["JCertPreApplication.API/JCertPreApplication.API.csproj", "JCertPreApplication.API/"]
COPY ["JCertPreApplication.Application/JCertPreApplication.Application.csproj", "JCertPreApplication.Application/"]
COPY ["JCertPreApplication.Domain/JCertPreApplication.Domain.csproj", "JCertPreApplication.Domain/"]
COPY ["JCertPreApplication.Persistence/JCertPreApplication.Persistence.csproj", "JCertPreApplication.Persistence/"]

# Restore dependencies
RUN dotnet restore "JCertPreApplication.API/JCertPreApplication.API.csproj"

# Copy the rest of the source code
COPY . .

# Build the application
RUN dotnet build "JCertPreApplication.API/JCertPreApplication.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "JCertPreApplication.API/JCertPreApplication.API.csproj" -c Release -o /app/publish

# Final stage
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS final
WORKDIR /app

# Install curl for healthcheck
RUN apt-get update && apt-get install -y curl && rm -rf /var/lib/apt/lists/*

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Create directory for environment files
RUN mkdir -p /app/config

# Add healthcheck
HEALTHCHECK --interval=30s --timeout=3s --retries=3 \
    CMD curl -f http://localhost:5001/health || exit 1

# Expose ports
EXPOSE 5001 7001

# Set entry point
ENTRYPOINT ["dotnet", "JCertPreApplication.API.dll"] 