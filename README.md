# JCertPre Backend Application

## 🚀 Quick Start

### 1. Setup Environment Variables

```bash
# Copy template
cp env.example .env

# Edit .env with your database info and JWT secrets (minimum 32 characters each)
```

### 2. Required Environment Variables

```bash
# Database
JCERTPRE_DB_CONNECTION_STRING=Server=YOUR_SERVER;Database=JCertPreDB;User ID=sa;Password=YOUR_PASSWORD;TrustServerCertificate=True

# JWT (minimum 32 characters each for security)
JWT_SECRET_KEY=your-super-secret-jwt-key-at-least-32-characters-long-for-security
JWT_REFRESH_SECRET_KEY=your-refresh-secret-key-also-at-least-32-characters-long
JWT_ISSUER=JCertPre-API
JWT_AUDIENCE=JCertPre-Client
JWT_EXPIRY_MINUTES=60
```

### 3. Run Application

```bash
dotnet restore
dotnet run --project JCertPreApplication.API
```

Application sẽ hiển thị status của configuration:
```
✅ Loaded .env file successfully
✅ Database connection configured successfully
🔧 Configuration Status:
   Database: ✅ Configured
   JWT Secret: ✅ Configured
   Environment: Development
```

## 📚 Documentation

- [Environment Setup Guide](./ENVIRONMENT_SETUP.md) - Chi tiết cách setup environment variables
- API Documentation: https://localhost:7001/swagger (khi chạy ở development mode)

## 🏗️ Project Structure

```
├── JCertPreApplication.API/          # Web API Layer
├── JCertPreApplication.Application/  # Application Layer (Services, DTOs)
├── JCertPreApplication.Domain/       # Domain Layer (Entities, Enums)
├── JCertPreApplication.Persistence/  # Data Access Layer (DbContext, Configurations)
└── env.example                       # Environment variables template
```

## 🔐 Security

- **NEVER** commit `.env` files to Git (already in .gitignore)
- **NEVER** put sensitive data in `appsettings.json`
- Use strong JWT secrets (minimum 32 characters, recommended 64+)
- Include uppercase, lowercase, numbers, special characters in secrets
- Share `.env` file với team qua secure channels
- Use Azure Key Vault or similar for production

## 🔧 Development Setup

1. **Install Prerequisites:**
   - .NET 8 SDK
   - SQL Server (or Docker)

2. **Setup Project:**
   ```bash
   git clone <repository>
   cd JCertPre-BE
   cp env.example .env
   ```

3. **Configure .env:**
   - Add your database connection string
   - Create strong JWT secrets (minimum 32 characters each)
   - Verify all required variables are set

4. **Run:**
   ```bash
   dotnet restore
   dotnet ef database update --project JCertPreApplication.API  # (after creating migrations)
   dotnet run --project JCertPreApplication.API
   ```

## 👥 Team Workflow

- **New team member:** Get `.env` file from team lead via secure channel
- **Configuration changes:** Update `env.example` and notify team  
- **All secrets:** Managed in `.env` file only (no other methods)

## 🔒 JWT Secrets Examples

**Strong secrets (recommended):**
```bash
JWT_SECRET_KEY=Kj8#mP9$xL2@nQ5^wR7&vT4!uY6*sE8(
JWT_REFRESH_SECRET_KEY=zN3%gH6@kM9$pL2^fD5!cV8&xB1*qW4#
```

## 🐛 Troubleshooting

| Issue | Solution |
|-------|----------|
| `.env file not found` | Run `cp env.example .env` |
| `JWT_SECRET_KEY not found` | Add JWT secrets to .env (min 32 chars) |
| `JWT secret too short` | Use at least 32 characters for each JWT secret |
| `Database connection failed` | Check `JCERTPRE_DB_CONNECTION_STRING` in .env |
| Application won't start | Check console for missing environment variables | 