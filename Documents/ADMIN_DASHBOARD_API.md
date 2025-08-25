# Admin Dashboard API

This document describes analytics and reporting endpoints for the Admin Dashboard.

---

## Table of Contents

1. [Overview](#overview)
2. [Endpoints](#endpoints)
   - [GET /api/admin-dashboard/revenue/total](#get-api-admin-dashboard-revenue-total)
   - [GET /api/admin-dashboard/enrollments/total](#get-api-admin-dashboard-enrollments-total)
   - [GET /api/admin-dashboard/enrollments/current-month](#get-api-admin-dashboard-enrollments-current-month)
   - [GET /api/admin-dashboard/revenue/current-month](#get-api-admin-dashboard-revenue-current-month)
   - [GET /api/admin-dashboard/enrollments/by-month](#get-api-admin-dashboard-enrollments-by-month)
   - [GET /api/admin-dashboard/revenue/by-month](#get-api-admin-dashboard-revenue-by-month)
3. [Status Codes](#status-codes)
4. [Error Handling](#error-handling)
5. [Implementation Structure](#implementation-structure)
6. [Dependency Injection](#dependency-injection)
7. [Testing the API](#testing-the-api)
8. [Future Extensions](#future-extensions)

---

## Overview

This API provides analytics and reporting capabilities for the Admin Dashboard.

---

## Endpoints

### GET /api/admin-dashboard/revenue/total
Retrieve the total revenue from deposit transactions.

- Sum the amount of transactions where `PaymentType = Money` and `PaymentStatus = Completed`
- Returns total amount, number of transactions, and calculation timestamp

Response:
```json
{
  "totalAmount": 1500000,
  "currency": "VND",
  "totalTransactions": 25,
  "calculatedAt": "2025-08-09T10:30:00Z"
}
```

---

### GET /api/admin-dashboard/enrollments/total
Retrieve the total number of course enrollments.

- Count total records in `Enrollments`
- Returns total count and calculation timestamp

Response:
```json
{
  "totalCount": 150,
  "calculatedAt": "2025-08-09T10:30:00Z"
}
```

---

### GET /api/admin-dashboard/enrollments/current-month
Retrieve the number of course enrollments in the current month.

- Count enrollments from the first day of the current month
- Returns count, current month, and calculation timestamp

Response:
```json
{
  "count": 23,
  "month": "08/2025",
  "calculatedAt": "2025-08-09T10:30:00Z"
}
```

---

### GET /api/admin-dashboard/revenue/current-month
Retrieve the revenue in the current month.

- Sum revenue from successful deposit transactions from the first day of the current month
- Returns total amount, currency, current month, and calculation timestamp

Response:
```json
{
  "totalAmount": 250000,
  "currency": "VND",
  "month": "08/2025",
  "calculatedAt": "2025-08-09T10:30:00Z"
}
```

---

### GET /api/admin-dashboard/enrollments/by-month
Retrieve monthly enrollment statistics for the last 12 months.

- Fetch enrollments for the last 12 months (from the current month backwards)
- Returns a dictionary where key is "MM/yyyy" and value is the number of enrollments
- Months without data will have value 0

Response:
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

---

### GET /api/admin-dashboard/revenue/by-month
Retrieve monthly revenue statistics for the last 12 months.

- Fetch revenue from successful deposit transactions for the last 12 months
- Returns a dictionary where key is "MM/yyyy" and value is the total revenue
- Months without data will have value 0

Response:
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

---

## Status Codes
- `200 OK`: Success
- `500 Internal Server Error`: Server error (uses `ApiException.InternalServerError`)

---

## Error Handling
The API uses a standardized `ApiException` system for error handling.

Service layer:
- Catch unexpected exceptions
- Revenue Total: Throw `ApiException.InternalServerError` with code `REVENUE_CALCULATION_ERROR`
- Enrollments Total: Throw `ApiException.InternalServerError` with code `ENROLLMENTS_COUNT_ERROR`
- Enrollments By Month: Throw `ApiException.InternalServerError` with code `ENROLLMENTS_BY_MONTH_ERROR`
- Current Month Enrollments: Throw `ApiException.InternalServerError` with code `CURRENT_MONTH_ENROLLMENTS_ERROR`
- Current Month Revenue: Throw `ApiException.InternalServerError` with code `CURRENT_MONTH_REVENUE_ERROR`
- Revenue By Month: Throw `ApiException.InternalServerError` with code `REVENUE_BY_MONTH_ERROR`

Controller layer:
- No try-catch needed since Global Exception Handling Middleware will handle it
- `ApiException` is automatically converted to a standardized HTTP response

Error response format:
```json
{
  "statusCode": 500,
  "errorCode": "REVENUE_CALCULATION_ERROR",
  "message": "An error occurred while calculating total revenue. Please try again later.",
  "details": null,
  "path": "/api/admin-dashboard/revenue/total"
}
```

---

## Implementation Structure

1) Controller Layer
- File: `JCertPreApplication.API/Controllers/AdminDashboardController.cs`
- Responsibility: Handle HTTP requests and responses

2) Service Layer
- Interface: `JCertPreApplication.Application/Features/AdminDashboard/IAdminDashboardService.cs`
- Implementation: `JCertPreApplication.Application/Features/AdminDashboard/AdminDashboardService.cs`
- Responsibility: Business logic

3) Repository Layer
- Updated: `IPaymentRepository.cs` and `PaymentRepository.cs`
- New method: `GetPaymentsByTypeAsync(PaymentType paymentType)`
- Updated: `IEnrollmentRepository.cs` and `EnrollmentRepository.cs`
- New method: `GetTotalEnrollmentsCountAsync()`

4) DTOs
- `JCertPreApplication.Application/Dtos/AdminDashboard/TotalRevenueDto.cs`
- `JCertPreApplication.Application/Dtos/AdminDashboard/TotalEnrollmentsDto.cs`
- `JCertPreApplication.Application/Dtos/AdminDashboard/EnrollmentsByMonthDto.cs`
- `JCertPreApplication.Application/Dtos/AdminDashboard/CurrentMonthEnrollmentsDto.cs`
- `JCertPreApplication.Application/Dtos/AdminDashboard/CurrentMonthRevenueDto.cs`
- `JCertPreApplication.Application/Dtos/AdminDashboard/RevenueByMonthDto.cs`

---

## Dependency Injection

Service registration in `DependencyInjection.cs`:
```csharp
services.AddScoped<IAdminDashboardService, AdminDashboardService>();
```

---

## Testing the API

Use Swagger UI or Postman to test endpoints:
```
GET https://localhost:7001/api/admin-dashboard/revenue/total
GET https://localhost:7001/api/admin-dashboard/enrollments/total
GET https://localhost:7001/api/admin-dashboard/enrollments/current-month
GET https://localhost:7001/api/admin-dashboard/revenue/current-month
GET https://localhost:7001/api/admin-dashboard/enrollments/by-month
GET https://localhost:7001/api/admin-dashboard/revenue/by-month
```

---

## Future Extensions

Potential additional endpoints:
- `/api/admin-dashboard/users/statistics`
- `/api/admin-dashboard/courses/analytics`
- `/api/admin-dashboard/top-courses`
- `/api/admin-dashboard/users/by-month`
- `/api/admin-dashboard/payments/by-status`
- `/api/admin-dashboard/enrollment-revenue-comparison`
