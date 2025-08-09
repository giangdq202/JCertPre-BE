# Admin Dashboard API

## Mô tả
API này cung cấp các chức năng thống kê và báo cáo dành cho Admin Dashboard.

## Endpoints

### GET /api/admin-dashboard/revenue/total
Lấy thống kê tổng doanh thu từ các giao dịch nạp tiền.

**Mô tả:**
- Tính tổng số tiền từ các giao dịch có `PaymentType = Money` và `PaymentStatus = Completed`
- Trả về tổng số tiền, số lượng giao dịch và timestamp

**Response:**
```json
{
  "totalAmount": 1500000,
  "currency": "VND",
  "totalTransactions": 25,
  "calculatedAt": "2025-08-09T10:30:00Z"
}
```

**Status Codes:**
- `200 OK`: Thành công
- `500 Internal Server Error`: Lỗi server (sử dụng `ApiException.InternalServerError`)

## Error Handling
API sử dụng hệ thống `ApiException` để xử lý lỗi một cách chuẩn xác:

**Trong Service Layer:**
- Catch tất cả exceptions không mong muốn
- Throw `ApiException.InternalServerError` với error code `REVENUE_CALCULATION_ERROR`

**Trong Controller Layer:**
- Không cần try-catch vì Global Exception Handling Middleware sẽ xử lý
- `ApiException` được tự động convert thành HTTP response với format chuẩn

**Error Response Format:**
```json
{
  "statusCode": 500,
  "errorCode": "REVENUE_CALCULATION_ERROR",
  "message": "An error occurred while calculating total revenue. Please try again later.",
  "details": null,
  "path": "/api/admin-dashboard/revenue/total"
}
```

## Cấu trúc Implementation

### 1. Controller Layer
- **File:** `JCertPreApplication.API/Controllers/AdminDashboardController.cs`
- **Chức năng:** Xử lý HTTP requests và responses

### 2. Service Layer
- **Interface:** `JCertPreApplication.Application/Features/AdminDashboard/IAdminDashboardService.cs`
- **Implementation:** `JCertPreApplication.Application/Features/AdminDashboard/AdminDashboardService.cs`
- **Chức năng:** Xử lý business logic

### 3. Repository Layer
- **Updated:** `IPaymentRepository.cs` và `PaymentRepository.cs`
- **Method mới:** `GetPaymentsByTypeAsync(PaymentType paymentType)`

### 4. DTO
- **File:** `JCertPreApplication.Application/Dtos/AdminDashboard/TotalRevenueDto.cs`
- **Chức năng:** Định nghĩa cấu trúc dữ liệu trả về

## Dependency Injection
Service đã được đăng ký trong `DependencyInjection.cs`:
```csharp
services.AddScoped<IAdminDashboardService, AdminDashboardService>();
```

## Test API
Sử dụng Swagger UI hoặc Postman để test endpoint:
```
GET https://localhost:7001/api/admin-dashboard/revenue/total
```

## Mở rộng tương lai
Có thể dễ dàng thêm các endpoints khác như:
- `/api/admin-dashboard/users/statistics`
- `/api/admin-dashboard/courses/analytics`
- `/api/admin-dashboard/revenue/by-period`
