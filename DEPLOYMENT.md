# JCertPre-BE Deployment Guide

## 1. Trên máy Local (Build và Push Image)

### Build Image
```powershell
docker build -t your-dockerhub-username/jcertpre-be:latest -f Dockerfile .
```

### Đăng nhập Docker Hub
```powershell
docker login
```

### Push Image lên Docker Hub
```powershell
docker push your-dockerhub-username/jcertpre-be:latest
```

## 2. Trên Server Deploy

### Chuẩn bị files cần thiết
- Copy file `docker-compose.yml` lên server
- Copy file `.env` lên server và cập nhật với cấu hình production

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

## 3. Quy trình cập nhật

1. **Local**: Build và push image mới với tag `latest`
2. **Server**: Watchtower sẽ tự động pull image mới và restart container

## 4. Lưu ý

- Thay `your-dockerhub-username` trong `docker-compose.yml` bằng username Docker Hub thực tế
- Cập nhật file `.env` với cấu hình production phù hợp
- Watchtower kiểm tra image mới mỗi 5 phút (300 giây)
