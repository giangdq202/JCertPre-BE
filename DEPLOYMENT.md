# JCertPre-BE Deployment Guide

## 1. Trên máy Local (Build và Push Image)

### Build Image với version tag
```powershell
# Build với version cụ thể (khuyến nghị)
docker build -t wakiwaki2922/jcertpre-be:v1.0 -f Dockerfile .

# Hoặc build với latest (để backup)
docker build -t wakiwaki2922/jcertpre-be:latest -f Dockerfile .
```

### Đăng nhập Docker Hub
```powershell
docker login
```

### Push Image lên Docker Hub
```powershell
# Push version cụ thể
docker push wakiwaki2922/jcertpre-be:v1.0

# Push latest (nếu có)
docker push wakiwaki2922/jcertpre-be:latest
```

## 2. Trên Server Deploy

### Chuẩn bị files cần thiết
1. Copy file `docker-compose.yml` lên server
2. Tạo file `.env` trên server với cấu hình production

#### File `.env` mẫu cho production:
```properties
# Database Configuration
ConnectionStrings__JCertPreDB="Host=YOUR_DB_HOST;Database=JCertPreDB;Username=admin;Password=YOUR_PASSWORD;Port=5432"

# JWT Configuration
Jwt__SecretKey="your-super-secret-jwt-key-at-least-32-characters-long"
Jwt__RefreshSecretKey="your-refresh-secret-key-also-at-least-32-characters-long"
Jwt__Issuer="JCertPre-API"
Jwt__Audience="JCertPre-Client"
Jwt__ExpiryInMinutes="60"

# Redis Configuration
Redis__ConnectionString="YOUR_REDIS_HOST:6379"

# API Configuration
Api__Environment="Production"
Api__Urls="https://YOUR_DOMAIN:5001"

# CORS Configuration
Cors__AllowedOrigins="https://YOUR_FRONTEND_DOMAIN"

# Cloudinary Configuration
Cloudinary__CloudName="your-cloudinary-cloud-name"
Cloudinary__ApiKey="your-cloudinary-api-key"
Cloudinary__ApiSecret="your-cloudinary-api-secret"
Cloudinary__Secure="true"

# Firebase Configuration
Firebase__Type="service_account"
Firebase__ProjectId="your-firebase-project-id"
Firebase__PrivateKeyId="your-private-key-id"
Firebase__PrivateKey="-----BEGIN PRIVATE KEY-----\nyour-private-key-content\n-----END PRIVATE KEY-----"
Firebase__ClientEmail="your-client-email@your-project.iam.gserviceaccount.com"
Firebase__ClientId="your-client-id"
Firebase__AuthUri="https://accounts.google.com/o/oauth2/auth"
Firebase__TokenUri="https://oauth2.googleapis.com/token"
Firebase__AuthProviderX509CertUrl="https://www.googleapis.com/oauth2/v1/certs"
Firebase__ClientX509CertUrl="https://www.googleapis.com/robot/v1/metadata/x509/your-client-email"
Firebase__UniverseDomain="googleapis.com"

# LiveKit Configuration
LiveKit__ApiKey="your-livekit-api-key"
LiveKit__ApiSecret="your-livekit-api-secret"
```

### Khởi chạy ứng dụng
```bash
docker-compose up -d
```

### Kiểm tra logs
```bash
docker-compose logs -f jcertpre-be
```

### Dừng ứng dụng
```bash
docker-compose down
```

## 3. Quy trình cập nhật với Image Versioning

### Phát hành phiên bản mới:

1. **Local**: Build và push image với version mới
```powershell
# Ví dụ: từ v1.0 lên v1.1
docker build -t wakiwaki2922/jcertpre-be:v1.1 -f Dockerfile .
docker push wakiwaki2922/jcertpre-be:v1.1
```

2. **Server**: Cập nhật version trong docker-compose.yml
```bash
# Sửa file docker-compose.yml
nano docker-compose.yml

# Thay đổi:
# image: wakiwaki2922/jcertpre-be:v1.0
# Thành:
# image: wakiwaki2922/jcertpre-be:v1.1
```

3. **Deploy phiên bản mới:**
```bash
# Pull image mới và restart
docker-compose pull
docker-compose up -d

# Hoặc force recreate container
docker-compose up -d --force-recreate
```

### Rollback về phiên bản cũ:
```bash
# Sửa docker-compose.yml về version cũ
# image: wakiwaki2922/jcertpre-be:v1.0

# Restart với version cũ
docker-compose up -d --force-recreate
```

## 4. Lưu ý

- **Sử dụng version tags**: Thay vì `latest`, sử dụng version cụ thể như `v1.0`, `v1.1` để dễ quản lý
- **File `.env`**: Chứa thông tin nhạy cảm, đảm bảo chmod 600 cho file này
- **Backup**: Luôn giữ lại version cũ để có thể rollback nếu cần
- **Testing**: Test kỹ trước khi deploy version mới lên production
- **Database migrations**: Đảm bảo database đã được migrate trước khi deploy
- **Dependencies**: Đảm bảo database và Redis server đã được khởi tạo trước khi chạy ứng dụng

## 5. Version Management Best Practices

### Semantic Versioning:
- `v1.0.0` - Major version (breaking changes)
- `v1.1.0` - Minor version (new features)  
- `v1.1.1` - Patch version (bug fixes)

### Git Tags:
```bash
# Tạo git tag trước khi build
git tag v1.1.0
git push origin v1.1.0

# Build image với cùng version
docker build -t wakiwaki2922/jcertpre-be:v1.1.0 -f Dockerfile .
```

## 6. Troubleshooting

### Kiểm tra environment variables trong container:
```bash
docker exec jcertpre-be-container printenv | grep -E "ConnectionStrings|Jwt|Redis"
```

### Kiểm tra kết nối database:
```bash
docker-compose logs jcertpre-be | grep -i "database\|connection"
```

### Kiểm tra image version hiện tại:
```bash
docker images wakiwaki2922/jcertpre-be
```

### Xem container đang chạy version nào:
```bash
docker inspect jcertpre-be-container | grep "Image"
```

### Restart container manually:
```bash
docker-compose restart jcertpre-be
```

### Xóa container và tạo lại:
```bash
docker-compose down
docker-compose up -d
```
