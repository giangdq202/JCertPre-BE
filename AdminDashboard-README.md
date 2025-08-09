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

### GET /api/admin-dashboard/enrollments/total
Lấy thống kê tổng số lượt đăng ký khóa học.

**Mô tả:**
- Đếm tổng số bản ghi trong bảng `Enrollments`
- Trả về tổng số lượt đăng ký và timestamp

**Response:**
```json
{
  "totalCount": 150,
  "calculatedAt": "2025-08-09T10:30:00Z"
}
```

### GET /api/admin-dashboard/enrollments/by-month
Lấy thống kê số lượt đăng ký khóa học theo từng tháng trong 12 tháng gần nhất.

**Mô tả:**
- Lấy dữ liệu enrollment của 12 tháng gần nhất (từ tháng hiện tại lùi về quá khứ)
- Trả về dữ liệu dạng Dictionary với key là "MM/yyyy" và value là số lượt đăng ký
- Các tháng không có dữ liệu sẽ có giá trị 0

**Response:**
```json
{
  "data": {
    "09/2024": 12,
    "10/2024": 25,
    "11/2024": 18,
    "12/2024": 30,
    "01/2025": 22,
    "02/2025": 15,
    "03/2025": 28,
    "04/2025": 35,
    "05/2025": 20,
    "06/2025": 27,
    "07/2025": 32,
    "08/2025": 19
  },
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
- Revenue API: Throw `ApiException.InternalServerError` với error code `REVENUE_CALCULATION_ERROR`
- Enrollments Total API: Throw `ApiException.InternalServerError` với error code `ENROLLMENTS_COUNT_ERROR`
- Enrollments By Month API: Throw `ApiException.InternalServerError` với error code `ENROLLMENTS_BY_MONTH_ERROR`

**Trong Controller Layer:**
- Không cần try-catch vì Global Exception Handling Middleware sẽ xử lý
- `ApiException` được tự động convert thành HTTP response với format chuẩn

**Error Response Format:**
```json
{
  "statusCode": 500,
  "errorCode": "REVENUE_CALCULATION_ERROR", // hoặc "ENROLLMENTS_COUNT_ERROR" hoặc "ENROLLMENTS_BY_MONTH_ERROR"
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
- **Updated:** `IEnrollmentRepository.cs` và `EnrollmentRepository.cs`
- **Method mới:** `GetTotalEnrollmentsCountAsync()`

### 4. DTO
- **File:** `JCertPreApplication.Application/Dtos/AdminDashboard/TotalRevenueDto.cs`
- **File:** `JCertPreApplication.Application/Dtos/AdminDashboard/TotalEnrollmentsDto.cs`
- **File:** `JCertPreApplication.Application/Dtos/AdminDashboard/EnrollmentsByMonthDto.cs`
- **Chức năng:** Định nghĩa cấu trúc dữ liệu trả về

## Dependency Injection
Service đã được đăng ký trong `DependencyInjection.cs`:
```csharp
services.AddScoped<IAdminDashboardService, AdminDashboardService>();
```

## Test API
Sử dụng Swagger UI hoặc Postman để test endpoints:
```
GET https://localhost:7001/api/admin-dashboard/revenue/total
GET https://localhost:7001/api/admin-dashboard/enrollments/total
GET https://localhost:7001/api/admin-dashboard/enrollments/by-month
```

## Mở rộng tương lai
Có thể dễ dàng thêm các endpoints khác như:
- `/api/admin-dashboard/users/statistics`
- `/api/admin-dashboard/courses/analytics`
- `/api/admin-dashboard/revenue/by-period`
- `/api/admin-dashboard/enrollments/by-course`
- `/api/admin-dashboard/revenue/by-month` (tương tự enrollments by month)
- `/api/admin-dashboard/top-courses` (khóa học có nhiều lượt đăng ký nhất)
