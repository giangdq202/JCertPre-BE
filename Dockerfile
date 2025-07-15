#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 5001
EXPOSE 7001

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["JCertPreApplication.sln", "."]
COPY ["JCertPreApplication.API/JCertPreApplication.API.csproj", "JCertPreApplication.API/"]
COPY ["JCertPreApplication.Application/JCertPreApplication.Application.csproj", "JCertPreApplication.Application/"]
COPY ["JCertPreApplication.Application.Tests/JCertPreApplication.Application.Tests.csproj", "JCertPreApplication.Application.Tests/"]
COPY ["JCertPreApplication.Domain/JCertPreApplication.Domain.csproj", "JCertPreApplication.Domain/"]
COPY ["JCertPreApplication.Persistence/JCertPreApplication.Persistence.csproj", "JCertPreApplication.Persistence/"]
RUN dotnet restore "./JCertPreApplication.sln"
COPY . .
WORKDIR "/src/."
RUN dotnet build "./JCertPreApplication.sln" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./JCertPreApplication.API/JCertPreApplication.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JCertPreApplication.API.dll"] 