# Stage 1: Base - Lớp runtime tối ưu cho ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 5001
USER app

# Stage 2: Build - Lớp chứa SDK để build ứng dụng
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Đảm bảo NuGet cache được clear và cấu hình đúng
RUN dotnet nuget locals all --clear

# Tối ưu hóa caching bằng cách restore dependencies trước
COPY ["JCertPreApplication.sln", "."]
COPY ["JCertPreApplication.API/JCertPreApplication.API.csproj", "JCertPreApplication.API/"]
COPY ["JCertPreApplication.Application/JCertPreApplication.Application.csproj", "JCertPreApplication.Application/"]
COPY ["JCertPreApplication.Domain/JCertPreApplication.Domain.csproj", "JCertPreApplication.Domain/"]
COPY ["JCertPreApplication.Persistence/JCertPreApplication.Persistence.csproj", "JCertPreApplication.Persistence/"]

# Restore với verbose để debug nếu cần
RUN dotnet restore "JCertPreApplication.sln" --verbosity normal

# Copy toàn bộ source code và build
COPY . .
WORKDIR "/src"

# Build solution (bao gồm restore tự động)
RUN dotnet build "JCertPreApplication.sln" -c $BUILD_CONFIGURATION

# Stage 3: Publish - Chỉ publish project API
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "JCertPreApplication.API/JCertPreApplication.API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Stage 4: Final - Lớp cuối cùng, nhẹ nhất
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "JCertPreApplication.API.dll"] 