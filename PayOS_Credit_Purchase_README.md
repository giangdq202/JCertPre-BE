# PayOS Credit Purchase Implementation

## Tổng quan

Đã triển khai thành công tính năng nạp credit cho user sử dụng PayOS với rate 1:1 (1 credit = 1 VND).

## Các file đã thêm/sửa

### 1. DTOs mới
- `JCertPreApplication.Application/Dtos/Payment/CreditPurchaseDtos.cs`
  - `CreateCreditPurchaseRequestDto`: Request từ client
  - `CreateCreditPurchaseResponseDto`: Response trả về link thanh toán
  - `ConfirmWebhookRequestDto`: Request để đăng ký webhook

### 2. Service Layer
- **IPaymentService**: Thêm 3 methods mới
  - `CreateCreditPurchaseAsync()`: Tạo link thanh toán
  - `ProcessPayOSWebhookAsync()`: Xử lý webhook
  - `ConfirmPayOSWebhookAsync()`: Đăng ký webhook URL

- **PaymentService**: Implement các methods mới với logic:
  - Validation user tồn tại
  - Tạo orderCode unique
  - Gọi PayOS service để tạo payment link
  - Xử lý webhook và cập nhật credit
  - Ghi lại credit transaction

### 3. Controller Layer
- **PaymentController**: Thêm 3 endpoints mới
  - `POST /api/payment/create-credit-purchase`: Tạo thanh toán
  - `POST /api/payment/payos-webhook`: Webhook từ PayOS
  - `POST /api/payment/confirm-webhook`: Đăng ký webhook

### 4. Configuration
- **PayOSConfiguration**: Thêm ReturnUrl và CancelUrl
- **PayOSService**: Cập nhật để sử dụng URLs từ configuration thay vì hardcode
- **env.example**: Thêm PayOS__ReturnUrl và PayOS__CancelUrl
- **appsettings.json**: Cập nhật PayOS section

## API Endpoints

### 1. Tạo giao dịch nạp credit
```http
POST /api/payment/create-credit-purchase
Content-Type: application/json

{
  "userId": "550e8400-e29b-41d4-a716-446655440000",
  "creditAmount": 100
}
```

**Response:**
```json
{
  "paymentUrl": "https://pay.payos.vn/web/...",
  "orderCode": 1722470400123456,
  "amount": 100,
  "description": "Nap 100 credit cho John Doe"
}
```

### 2. Webhook từ PayOS (tự động gọi)
```http
POST /api/payment/payos-webhook
Content-Type: application/json

{
  "code": "00",
  "desc": "Success",
  "success": true,
  "data": {
    "orderCode": 1722470400123456,
    "amount": 100,
    "description": "Nap 100 credit cho John Doe",
    "accountNumber": "12345678",
    "reference": "FT22197...",
    // ... other fields
  },
  "signature": "..."
}
```

### 3. Đăng ký webhook URL (chỉ chạy 1 lần)
```http
POST /api/payment/confirm-webhook
Content-Type: application/json

{
  "webhookUrl": "https://your-api-domain.com/api/payment/payos-webhook"
}
```

## Luồng hoạt động

1. **Client** gọi API `create-credit-purchase` với userId và creditAmount
2. **Backend** tạo Payment record với status Pending
3. **Backend** gọi PayOS để tạo payment link và trả về cho client
4. **Client** redirect user đến PayOS để thanh toán
5. **User** thực hiện thanh toán trên PayOS
6. **PayOS** gửi webhook về server khi có kết quả
7. **Backend** xác thực webhook, cập nhật credit user và ghi transaction

## Cấu hình cần thiết

1. **PayOS Credentials**: Cập nhật trong `appsettings.json` hoặc environment variables
```json
{
  "PayOS": {
    "ClientId": "your-payos-client-id",
    "ApiKey": "your-payos-api-key", 
    "ChecksumKey": "your-payos-checksum-key",
    "ReturnUrl": "https://your-frontend.com/payment/success",
    "CancelUrl": "https://your-frontend.com/payment/cancel"
  }
}
```

**Hoặc sử dụng Environment Variables (khuyên dùng):**
```bash
PayOS__ClientId="your-payos-client-id"
PayOS__ApiKey="your-payos-api-key"
PayOS__ChecksumKey="your-payos-checksum-key"
PayOS__ReturnUrl="https://your-frontend.com/payment/success"
PayOS__CancelUrl="https://your-frontend.com/payment/cancel"
```

2. **Webhook Registration**: Chạy API confirm-webhook một lần để đăng ký
```bash
curl -X POST https://your-api-domain.com/api/payment/confirm-webhook \
  -H "Content-Type: application/json" \
  -d '{"webhookUrl": "https://your-api-domain.com/api/payment/payos-webhook"}'
```

## Database Changes

**Không có thay đổi schema!** 

Sử dụng các entities có sẵn:
- `Payment`: Lưu thông tin giao dịch (PaymentType = Money)
- `CreditTransaction`: Ghi lại lịch sử thay đổi credit
- `User`: Cập nhật field `credit`

## Lưu ý quan trọng

1. **Rate 1:1**: 1 credit = 1 VND
2. **PaymentType**: PayOS sử dụng `PaymentType.Money` (không phải Credit)
3. **PayOS Description Limit**: PayOS giới hạn description tối đa 25 ký tự. Đã implement helper method `CreatePayOSDescription()` để đảm bảo tuân thủ giới hạn này
4. **Idempotency**: Webhook có thể gửi nhiều lần, đã xử lý tránh duplicate
5. **Security**: Webhook được verify signature từ PayOS
6. **Logging**: Đầy đủ log cho debug và monitor
7. **Error Handling**: Webhook luôn trả 200 OK để PayOS không resend

## Testing

Để test tính năng:
1. Đảm bảo PayOS credentials đã được cấu hình
2. Đăng ký webhook URL
3. Gọi API create-credit-purchase
4. Thực hiện thanh toán trên PayOS (có thể dùng test card)
5. Kiểm tra credit user đã được cập nhật
6. Xem lịch sử giao dịch qua API credit-history

## Dependencies

Tất cả dependencies đã có sẵn:
- PayOS SDK đã được cài trong Persistence layer
- IPaymentGateway đã được implement bởi PayOSService
- Các repositories cần thiết đã có sẵn
