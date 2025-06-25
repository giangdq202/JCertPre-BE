# Environment Variables Setup

## 🔐 Bảo mật thông tin nhạy cảm

Project này sử dụng file `.env` trong thư mục `JCertPreApplication.API/` để quản lý tất cả thông tin nhạy cảm như connection strings, JWT secrets.

## 📋 Setup cho Development

### 1. Copy template và configure

```bash
# 1. Copy template file vào API directory
cp env.example JCertPreApplication.API/.env

# 2. Chỉnh sửa file .env với thông tin thực của bạn
```

### 2. File .env mẫu

```bash
# Database Configuration
JCERTPRE_DB_CONNECTION_STRING=Server=DESKTOP-JJTIOH3\SQLEXPRESS;User ID=sa;Password=12345;Database=JCertPreDB;Trusted_Connection=False;TrustServerCertificate=True

# JWT Configuration (minimum 32 characters each)
JWT_SECRET_KEY=your-super-secret-jwt-key-at-least-32-characters-long-for-security
JWT_REFRESH_SECRET_KEY=your-refresh-secret-key-also-at-least-32-characters-long
JWT_ISSUER=JCertPre-API
JWT_AUDIENCE=JCertPre-Client
JWT_EXPIRY_MINUTES=60

# API Configuration (optional)
ASPNETCORE_ENVIRONMENT=Development
ASPNETCORE_URLS=https://localhost:7001;http://localhost:5001
```

## 🚀 Setup cho Production

### 1. Azure App Service

```bash
# Set app settings
az webapp config appsettings set --resource-group myResourceGroup --name myAppName --settings \
  JCERTPRE_DB_CONNECTION_STRING="Server=..." \
  JWT_SECRET_KEY="..." \
  JWT_REFRESH_SECRET_KEY="..." \
  JWT_ISSUER="JCertPre-API" \
  JWT_AUDIENCE="JCertPre-Client"
```

### 2. Docker

```dockerfile
# Dockerfile
ENV JCERTPRE_DB_CONNECTION_STRING=""
ENV JWT_SECRET_KEY=""
ENV JWT_REFRESH_SECRET_KEY=""
ENV JWT_ISSUER=""
ENV JWT_AUDIENCE=""
```

```bash
# Docker run với env vars
docker run \
  -e JCERTPRE_DB_CONNECTION_STRING="..." \
  -e JWT_SECRET_KEY="..." \
  -e JWT_REFRESH_SECRET_KEY="..." \
  -e JWT_ISSUER="JCertPre-API" \
  -e JWT_AUDIENCE="JCertPre-Client" \
  myapp
```

### 3. Kubernetes

```yaml
# secret.yaml
apiVersion: v1
kind: Secret
metadata:
  name: jcertpre-secrets
type: Opaque
data:
  JCERTPRE_DB_CONNECTION_STRING: <base64-encoded-connection-string>
  JWT_SECRET_KEY: <base64-encoded-secret>
  JWT_REFRESH_SECRET_KEY: <base64-encoded-refresh-secret>

---
# configmap.yaml
apiVersion: v1
kind: ConfigMap
metadata:
  name: jcertpre-config
data:
  JWT_ISSUER: "JCertPre-API"
  JWT_AUDIENCE: "JCertPre-Client"
  JWT_EXPIRY_MINUTES: "60"
```

## 🔍 Environment Variables được sử dụng

| Variable | Required | Description | Example |
|----------|----------|-------------|---------|
| `JCERTPRE_DB_CONNECTION_STRING` | ✅ | Database connection string | `Server=localhost;Database=JCertPreDB;...` |
| `JWT_SECRET_KEY` | ✅ | JWT signing key (32+ chars) | `your-super-secret-jwt-key-at-least-32-characters-long` |
| `JWT_REFRESH_SECRET_KEY` | ✅ | JWT refresh token key (32+ chars) | `your-refresh-secret-key-also-at-least-32-characters` |
| `JWT_ISSUER` | ✅ | JWT issuer | `JCertPre-API` |
| `JWT_AUDIENCE` | ✅ | JWT audience | `JCertPre-Client` |
| `JWT_EXPIRY_MINUTES` | ❌ | Token expiry time | `60` (default) |
| `ASPNETCORE_ENVIRONMENT` | ❌ | Environment name | `Development/Production` |
| `ASPNETCORE_URLS` | ❌ | Bind URLs | `https://localhost:7001` |

## ⚠️ Security Notes

1. **KHÔNG BAO GIỜ** commit file `.env` vào Git
2. **KHÔNG BAO GIỜ** để thông tin nhạy cảm trong `appsettings.json`
3. JWT secrets phải có ít nhất 32 characters để đảm bảo bảo mật
4. Sử dụng secrets mạnh với ký tự đặc biệt, số, chữ hoa/thường
5. Rotate secrets định kỳ trong production
6. Sử dụng Azure Key Vault hoặc AWS Secrets Manager cho production

## 🧪 Testing

```bash
# Kiểm tra environment variables đã được load chưa
dotnet run --project JCertPreApplication.API

# Console sẽ hiển thị:
# ✅ Loaded .env file successfully
# ✅ Database connection configured successfully
# 🔧 Configuration Status:
#    Database: ✅ Configured
#    JWT Secret: ✅ Configured
#    Environment: Development
```

## 🔧 Troubleshooting

### Lỗi: ".env file not found"

```bash
# Copy template vào API directory và configure
cp env.example JCertPreApplication.API/.env
# Chỉnh sửa file .env với thông tin thực
```

### Lỗi: "JWT_SECRET_KEY environment variable not found"

```bash
# Trong file .env, đảm bảo có:
JWT_SECRET_KEY=your-super-secret-jwt-key-at-least-32-characters-long-for-security
JWT_REFRESH_SECRET_KEY=your-refresh-secret-key-also-at-least-32-characters-long
```

### Lỗi: "JWT_SECRET_KEY must be at least 32 characters long"

```bash
# Đảm bảo JWT secrets có ít nhất 32 ký tự:
JWT_SECRET_KEY=this-is-a-very-long-secret-key-with-more-than-32-characters
JWT_REFRESH_SECRET_KEY=this-is-another-very-long-refresh-secret-key-32-chars
```

### Lỗi: "JCERTPRE_DB_CONNECTION_STRING environment variable not found"

```bash
# Trong file .env:
JCERTPRE_DB_CONNECTION_STRING=Server=DESKTOP-JJTIOH3\SQLEXPRESS;User ID=sa;Password=12345;Database=JCertPreDB;Trusted_Connection=False;TrustServerCertificate=True
```

## 💡 Team Workflow

1. **Setup lần đầu:**
   ```bash
   git clone <repo>
   cp env.example JCertPreApplication.API/.env
   # Chỉnh sửa file .env với thông tin database và JWT secrets
   dotnet run --project JCertPreApplication.API
   ```

2. **Khi có team member mới:**
   - Share file `.env` qua secure channel (Slack DM, encrypted file, password manager)
   - Hoặc guide họ tạo file `.env` dựa trên `env.example`

3. **Khi thay đổi configuration:**
   - Update `env.example` với variables mới
   - Thông báo team update file `.env` của họ

## 🔒 JWT Secrets Guidelines

- **Minimum length:** 32 characters
- **Recommended length:** 64+ characters  
- **Include:** Uppercase, lowercase, numbers, special characters
- **Example strong secrets:**
  ```
  JWT_SECRET_KEY=Kj8#mP9$xL2@nQ5^wR7&vT4!uY6*sE8(
  JWT_REFRESH_SECRET_KEY=zN3%gH6@kM9$pL2^fD5!cV8&xB1*qW4#
  ```