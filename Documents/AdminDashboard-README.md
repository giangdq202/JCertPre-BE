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

### GET /api/admin-dashboard/enrollments/current-month
Lấy số lượt đăng ký khóa học trong tháng hiện tại.

**Mô tả:**
- Đếm số lượt đăng ký từ ngày đầu tháng hiện tại
- Trả về số lượng, tháng hiện tại và timestamp

**Response:**
```json
{
  "count": 23,
  "month": "08/2025",
  "calculatedAt": "2025-08-09T10:30:00Z"
}
```

### GET /api/admin-dashboard/revenue/current-month
Lấy doanh thu trong tháng hiện tại.

**Mô tả:**
- Tính tổng doanh thu từ các giao dịch nạp tiền thành công từ ngày đầu tháng hiện tại
- Trả về tổng số tiền, đơn vị tiền tệ, tháng hiện tại và timestamp

**Response:**
```json
{
  "totalAmount": 250000,
  "currency": "VND",
  "month": "08/2025",
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

### GET /api/admin-dashboard/revenue/by-month
Lấy thống kê doanh thu theo từng tháng trong 12 tháng gần nhất.

**Mô tả:**
- Lấy dữ liệu doanh thu từ các giao dịch nạp tiền thành công của 12 tháng gần nhất
- Trả về dữ liệu dạng Dictionary với key là "MM/yyyy" và value là tổng doanh thu
- Các tháng không có dữ liệu sẽ có giá trị 0

**Response:**
```json
{
  "data": {
    "09/2024": 1200000,
    "10/2024": 2500000,
    "11/2024": 0,
    "12/2024": 3000000,
    "01/2025": 2200000,
    "02/2025": 1500000,
    "03/2025": 2800000,
    "04/2025": 3500000,
    "05/2025": 2000000,
    "06/2025": 2700000,
    "07/2025": 3200000,
    "08/2025": 1900000
  },
  "currency": "VND",
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
- Revenue Total API: Throw `ApiException.InternalServerError` với error code `REVENUE_CALCULATION_ERROR`
- Enrollments Total API: Throw `ApiException.InternalServerError` với error code `ENROLLMENTS_COUNT_ERROR`
- Enrollments By Month API: Throw `ApiException.InternalServerError` với error code `ENROLLMENTS_BY_MONTH_ERROR`
- Current Month Enrollments API: Throw `ApiException.InternalServerError` với error code `CURRENT_MONTH_ENROLLMENTS_ERROR`
- Current Month Revenue API: Throw `ApiException.InternalServerError` với error code `CURRENT_MONTH_REVENUE_ERROR`
- Revenue By Month API: Throw `ApiException.InternalServerError` với error code `REVENUE_BY_MONTH_ERROR`

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
- **File:** `JCertPreApplication.Application/Dtos/AdminDashboard/CurrentMonthEnrollmentsDto.cs`
- **File:** `JCertPreApplication.Application/Dtos/AdminDashboard/CurrentMonthRevenueDto.cs`
- **File:** `JCertPreApplication.Application/Dtos/AdminDashboard/RevenueByMonthDto.cs`
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
GET https://localhost:7001/api/admin-dashboard/enrollments/current-month
GET https://localhost:7001/api/admin-dashboard/revenue/current-month
GET https://localhost:7001/api/admin-dashboard/enrollments/by-month
GET https://localhost:7001/api/admin-dashboard/revenue/by-month
```

## Mở rộng tương lai
Có thể dễ dàng thêm các endpoints khác như:
- `/api/admin-dashboard/users/statistics`
- `/api/admin-dashboard/courses/analytics`
- `/api/admin-dashboard/top-courses` (khóa học có nhiều lượt đăng ký nhất)
- `/api/admin-dashboard/users/by-month` (người dùng đăng ký theo tháng)
- `/api/admin-dashboard/payments/by-status` (thanh toán theo trạng thái)
- `/api/admin-dashboard/enrollment-revenue-comparison` (so sánh lượt đăng ký vs doanh thu)
