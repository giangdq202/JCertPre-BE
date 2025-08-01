# PayOS Integration Components - Ready for Implementation

This document describes the PayOS payment gateway components that have been implemented and are ready for integration into the JCertPre-BE application.

## Current Status

✅ **Implemented Components:**
- PayOS DTOs in Application layer
- IPaymentGateway interface 
- PayOSService implementation in Persistence layer
- PayOSConfiguration
- DependencyInjection setup

❌ **Not Integrated Yet:**
- PaymentService remains unchanged (credit payments only)
- PaymentController remains unchanged (credit endpoints only)

## Available Components

### 1. Application Layer (`JCertPreApplication.Application`)

#### DTOs (Data Transfer Objects)
- `PaymentDataDto`: Contains payment request information
- `CreatePaymentResultDto`: Contains payment link creation result
- `PaymentLinkInformationDto`: Contains payment link status and details
- `TransactionDto`: Contains transaction information
- `WebhookDataDto`: Contains verified webhook data
- `WebhookTypeDto`: Contains raw webhook data structure
- `ItemDataDto`: Contains item information for payment

#### Contracts
- `IPaymentGateway`: Interface defining payment gateway operations

### 2. Persistence Layer (`JCertPreApplication.Persistence`)

#### Services
- `PayOSService`: Complete implementation of `IPaymentGateway` using PayOS SDK
  - ✅ `CreatePaymentLinkAsync()`
  - ✅ `GetPaymentLinkInformationAsync()`
  - ✅ `CancelPaymentLinkAsync()`
  - ✅ `ConfirmWebhookAsync()`
  - ✅ `VerifyPaymentWebhookData()`

### 3. Domain Layer (`JCertPreApplication.Domain`)

#### Configuration
- `PayOSConfiguration`: Configuration model for PayOS settings (moved to Domain layer like other configs)

### 3. Dependency Injection
- `IPaymentGateway` is registered with `PayOSService` implementation in Persistence DI

## Current Architecture

```
Available but not used:
IPaymentGateway (PayOSService) ← Ready to be injected

Current unchanged:
PaymentController 
    ↓
IPaymentService (PaymentService) ← Only handles credit payments
```

## Integration Options

You can now integrate PayOS in several ways:

### Option 1: Extend PaymentService (Previous Approach)
- Add IPaymentGateway dependency to PaymentService
- Add external payment methods to IPaymentService
- Add external payment endpoints to PaymentController

### Option 2: Separate PayOS Service
- Create dedicated PayOSPaymentService using IPaymentGateway
- Create dedicated PayOSController 
- Keep PaymentService for credit payments only

### Option 3: Use PayOSService Directly
- Inject IPaymentGateway directly into controllers or other services
- Bypass additional abstraction layers

## Configuration

PayOS is configured and ready to use:

```json
{
  "PayOS": {
    "ClientId": "your-payos-client-id",
    "ApiKey": "your-payos-api-key",
    "ChecksumKey": "your-payos-checksum-key"
  }
}
```

## Usage Example

If you want to use PayOSService directly:

```csharp
[ApiController]
public class TestController : ControllerBase
{
    private readonly IPaymentGateway _paymentGateway;

    public TestController(IPaymentGateway paymentGateway)
    {
        _paymentGateway = paymentGateway; // Will inject PayOSService
    }

    [HttpPost("test-payos")]
    public async Task<IActionResult> TestPayOS()
    {
        var paymentData = new PaymentDataDto
        {
            OrderCode = DateTimeOffset.Now.ToUnixTimeSeconds(),
            Amount = 100000,
            Description = "Test payment",
            Items = new List<ItemDataDto>
            {
                new ItemDataDto { Name = "Test Item", Quantity = 1, Price = 100000 }
            },
            ReturnUrl = "https://example.com/success",
            CancelUrl = "https://example.com/cancel"
        };

        var result = await _paymentGateway.CreatePaymentLinkAsync(paymentData);
        return Ok(result);
    }
}
```

## Next Steps

The PayOS infrastructure is ready. You can now:

1. **Decide your integration approach** (extend PaymentService vs separate service vs direct usage)
2. **Implement business logic** around PayOS operations
3. **Add PayOS endpoints** to existing or new controllers
4. **Test with PayOS credentials**

## Files Ready for Integration

**Infrastructure (Ready):**
- ✅ `/Application/Dtos/Payment/` - All PayOS DTOs
- ✅ `/Application/Contracts/IPaymentGateway.cs`
- ✅ `/Persistence/Services/PayOSService.cs` 
- ✅ `/Persistence/Configuration/PayOSConfiguration.cs`
- ✅ PayOS package installed and configured

**Business Logic (Your Choice):**
- Where to add PayOS business logic?
- How to structure PayOS API endpoints?
- How to integrate with existing payment system?

The foundation is solid - now you can architect the business logic layer as you prefer! 🚀

## Setup Instructions

### 1. Configuration

Add PayOS configuration to your `appsettings.json`:

```json
{
  "PayOS": {
    "ClientId": "your-payos-client-id",
    "ApiKey": "your-payos-api-key",
    "ChecksumKey": "your-payos-checksum-key"
  }
}
```

Or use environment variables:
```bash
PayOS__ClientId="your-payos-client-id"
PayOS__ApiKey="your-payos-api-key"
PayOS__ChecksumKey="your-payos-checksum-key"
```

### 2. Dependency Injection

The services are registered in the DI container:

- **Application Layer**: `IPaymentService` → `PaymentService`
- **Persistence Layer**: `IPaymentGateway` → `PayOSService`

PaymentService automatically receives IPaymentGateway through constructor injection.

### 3. Package Dependencies

PayOS SDK is added to the Persistence layer:
- `PayOS` (version 1.0.9)

## API Endpoints

### Internal Credit Payments (Existing)
```http
GET /api/payment/history/{userId}
GET /api/payment/credit-history/{userId}
GET /api/payment/check-credit/{userId}/{amount}
```

### External Payment Gateway (New)

#### Create Payment Link
```http
POST /api/payment/external/create-link
Content-Type: application/json

{
  "orderCode": 1234567890,
  "amount": 100000,
  "description": "Payment for course enrollment",
  "items": [
    {
      "name": "Course XYZ",
      "quantity": 1,
      "price": 100000
    }
  ],
  "returnUrl": "https://yourapp.com/payment/success",
  "cancelUrl": "https://yourapp.com/payment/cancel"
}
```

#### Get Payment Information
```http
GET /api/payment/external/info/{orderId}
```

#### Cancel Payment
```http
PUT /api/payment/external/cancel/{orderId}?cancellationReason=User requested cancellation
```

#### Handle Webhook
```http
POST /api/payment/external/webhook
Content-Type: application/json

{
  "code": "00",
  "desc": "success",
  "success": true,
  "data": {
    "orderCode": 1234567890,
    "amount": 100000,
    // ... other webhook data
  },
  "signature": "webhook-signature"
}
```

## Usage Example

### In a Controller or Service

```csharp
[ApiController]
[Route("api/[controller]")]
public class CourseEnrollmentController : ControllerBase
{
    private readonly IPaymentService _paymentService;

    public CourseEnrollmentController(IPaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("enroll-with-external-payment")]
    public async Task<IActionResult> EnrollWithExternalPayment([FromBody] EnrollmentRequest request)
    {
        var paymentData = new PaymentDataDto
        {
            OrderCode = DateTimeOffset.Now.ToUnixTimeSeconds(),
            Amount = request.CoursePrice,
            Description = $"Enrollment for {request.CourseName}",
            Items = new List<ItemDataDto>
            {
                new ItemDataDto
                {
                    Name = request.CourseName,
                    Quantity = 1,
                    Price = request.CoursePrice
                }
            },
            ReturnUrl = "https://yourapp.com/enrollment/success",
            CancelUrl = "https://yourapp.com/enrollment/cancel"
        };

        // PaymentService will use IPaymentGateway (PayOSService) internally
        var result = await _paymentService.CreateExternalPaymentLinkAsync(paymentData);
        return Ok(result);
    }

    [HttpPost("enroll-with-credit")]
    public async Task<IActionResult> EnrollWithCredit([FromBody] CreditEnrollmentRequest request)
    {
        // PaymentService handles credit payments directly
        var result = await _paymentService.ProcessCreditPaymentAsync(
            request.UserId, 
            request.CourseId, 
            request.Amount, 
            $"Enrollment for course {request.CourseId}"
        );
        return Ok(result);
    }
}
```

## Key Design Principles

### 1. **Single Responsibility**
- `PayOSService`: Only handles PayOS SDK integration
- `PaymentService`: Handles business logic for all payment types
- `PaymentController`: Handles HTTP requests and responses

### 2. **Dependency Inversion**
- `PaymentService` depends on `IPaymentGateway` abstraction, not concrete PayOS implementation
- Easy to swap payment gateways (VNPay, Stripe, etc.) by implementing `IPaymentGateway`

### 3. **Open/Closed Principle**
- Can add new payment gateways without modifying existing code
- Business logic in PaymentService remains unchanged

### 4. **Clean Architecture Layers**
- Application layer contains business logic and abstractions
- Persistence layer contains external service implementations
- API layer contains presentation logic only

## Benefits of This Architecture

1. **Testability**: Easy to mock `IPaymentGateway` for unit testing `PaymentService`
2. **Flexibility**: Can switch payment providers without changing business logic
3. **Maintainability**: Clear separation of concerns
4. **Extensibility**: Easy to add new payment methods or gateways

## Error Handling

All payment operations include comprehensive error handling at the service level with proper HTTP status codes returned from the controller.

## Security Considerations

1. **Webhook Verification**: All webhook data is verified using PayOS signature verification
2. **Configuration Security**: Store PayOS credentials securely
3. **HTTPS**: Ensure all webhook URLs use HTTPS
4. **Validation**: Input validation is performed in PaymentService

## Testing

To test the PayOS integration:

1. Configure test credentials from PayOS dashboard
2. Use the `/api/payment/external/*` endpoints
3. Test webhook processing with PayOS test environment

## Troubleshooting

1. **Configuration Issues**: Check PayOS settings in appsettings.json
2. **DI Issues**: Ensure `IPaymentGateway` is properly registered
3. **Webhook Issues**: Verify webhook URL accessibility and signature verification

## Setup Instructions

### 1. Configuration

Add PayOS configuration to your `appsettings.json`:

```json
{
  "PayOS": {
    "ClientId": "your-payos-client-id",
    "ApiKey": "your-payos-api-key",
    "ChecksumKey": "your-payos-checksum-key"
  }
}
```

Or use environment variables:
```bash
PayOS__ClientId="your-payos-client-id"
PayOS__ApiKey="your-payos-api-key"
PayOS__ChecksumKey="your-payos-checksum-key"
```

### 2. Dependency Injection

The services are automatically registered in the DI container:

- **Application Layer**: `PayOSPaymentService` is registered as scoped
- **Persistence Layer**: `IPaymentGateway` is registered with `PayOSService` implementation

### 3. Package Dependencies

The following NuGet package is added to the Persistence layer:
- `PayOS` (version 1.0.9)

## API Endpoints

### Create Payment Link
```http
POST /api/PayOS/create-payment-link
Content-Type: application/json

{
  "orderCode": 1234567890,
  "amount": 100000,
  "description": "Payment for course enrollment",
  "items": [
    {
      "name": "Course XYZ",
      "quantity": 1,
      "price": 100000
    }
  ],
  "returnUrl": "https://yourapp.com/payment/success",
  "cancelUrl": "https://yourapp.com/payment/cancel"
}
```

### Get Payment Information
```http
GET /api/PayOS/payment-info/{orderId}
```

### Cancel Payment
```http
PUT /api/PayOS/cancel-payment/{orderId}?cancellationReason=User requested cancellation
```

### Confirm Webhook
```http
POST /api/PayOS/confirm-webhook
Content-Type: application/json

"https://yourapp.com/api/PayOS/webhook"
```

### Handle Webhook
```http
POST /api/PayOS/webhook
Content-Type: application/json

{
  "code": "00",
  "desc": "success",
  "success": true,
  "data": {
    "orderCode": 1234567890,
    "amount": 100000,
    "description": "Payment for course enrollment",
    "accountNumber": "12345678",
    "reference": "REF123456",
    "transactionDateTime": "2024-01-01 10:00:00",
    "currency": "VND",
    "paymentLinkId": "abc-def-ghi",
    "code": "00",
    "desc": "success",
    "counterAccountBankId": "970415",
    "counterAccountBankName": "Vietinbank",
    "counterAccountName": "NGUYEN VAN A",
    "counterAccountNumber": "987654321",
    "virtualAccountName": "CONG TY ABC",
    "virtualAccountNumber": "19036035456781"
  },
  "signature": "webhook-signature"
}
```

## Usage Example

### In a Controller or Service

```csharp
[ApiController]
[Route("api/[controller]")]
public class CourseEnrollmentController : ControllerBase
{
    private readonly PayOSPaymentService _payOSPaymentService;

    public CourseEnrollmentController(PayOSPaymentService payOSPaymentService)
    {
        _payOSPaymentService = payOSPaymentService;
    }

    [HttpPost("enroll")]
    public async Task<IActionResult> EnrollInCourse([FromBody] EnrollmentRequest request)
    {
        var paymentData = new PaymentDataDto
        {
            OrderCode = DateTimeOffset.Now.ToUnixTimeSeconds(),
            Amount = request.CoursePrice,
            Description = $"Enrollment for {request.CourseName}",
            Items = new List<ItemDataDto>
            {
                new ItemDataDto
                {
                    Name = request.CourseName,
                    Quantity = 1,
                    Price = request.CoursePrice
                }
            },
            ReturnUrl = "https://yourapp.com/enrollment/success",
            CancelUrl = "https://yourapp.com/enrollment/cancel"
        };

        var result = await _payOSPaymentService.CreatePaymentLinkAsync(paymentData);
        return Ok(result);
    }
}
```

## Error Handling

All PayOS operations include comprehensive error handling:

- **ArgumentException**: For invalid input parameters
- **Exception**: For PayOS SDK errors
- HTTP status codes are returned appropriately (400 for bad requests, 500 for server errors)

## Security Considerations

1. **Webhook Verification**: All webhook data is verified using PayOS signature verification
2. **Configuration Security**: Store PayOS credentials in environment variables or secure configuration
3. **HTTPS**: Ensure all webhook URLs use HTTPS
4. **Validation**: Input validation is performed on all payment requests

## Testing

To test the PayOS integration:

1. Obtain test credentials from PayOS dashboard
2. Configure the test credentials in your development environment
3. Use the provided API endpoints to create test payments
4. Use PayOS test environment for safe testing

## Monitoring and Logging

The implementation includes comprehensive logging:
- Payment creation attempts
- Webhook processing
- Error conditions
- Security events

Logs are written using the standard .NET ILogger interface and can be configured through `appsettings.json`.

## Troubleshooting

### Common Issues

1. **Configuration Not Found**: Ensure PayOS configuration is properly set in appsettings.json or environment variables
2. **Webhook Verification Failed**: Check that webhook signature verification is working correctly
3. **Payment Creation Failed**: Verify PayOS credentials and check network connectivity
4. **Invalid Order Code**: Ensure order codes are unique and within PayOS limits

### Debug Tips

1. Enable detailed logging for the PayOS namespace
2. Check PayOS dashboard for transaction status
3. Verify webhook URL is accessible from PayOS servers
4. Test with small amounts first

## Further Reading

- [PayOS Official Documentation](https://payos.vn/docs/)
- [PayOS .NET SDK](https://github.com/payOSHQ/payos-lib-dotnet)
- [Clean Architecture Principles](https://blog.cleancoder.com/uncle-bob/2012/08/13/the-clean-architecture.html)
