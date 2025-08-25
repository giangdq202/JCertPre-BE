using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Dtos.Payment;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.Payment;
using JCertPreApplication.Domain.Configuration;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Net;

namespace JCertPreApplication.UnitTests.Features.Payment;

public class PaymentServiceTests
{
    private readonly Mock<IPaymentRepository> _mockPaymentRepository;
    private readonly Mock<ICreditTransactionRepository> _mockCreditTransactionRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<ILogger<PaymentService>> _mockLogger;
    private readonly Mock<IPaymentGateway> _mockPaymentGateway;
    private readonly Mock<IOptions<FrontendConfiguration>> _mockFrontendConfig;
    private readonly PaymentService _paymentService;

    public PaymentServiceTests()
    {
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _mockCreditTransactionRepository = new Mock<ICreditTransactionRepository>();
        _mockUserRepository = new Mock<IUserRepository>();
        _mockLogger = new Mock<ILogger<PaymentService>>();
        _mockPaymentGateway = new Mock<IPaymentGateway>();
        _mockFrontendConfig = new Mock<IOptions<FrontendConfiguration>>();

        // Setup FrontendConfiguration
        var frontendConfig = new FrontendConfiguration
        {
            BaseUrl = "https://frontend.test",
            PaymentSuccessEndpoint = "/payment/success",
            PaymentErrorEndpoint = "/payment/error"
        };
        _mockFrontendConfig.Setup(x => x.Value).Returns(frontendConfig);

        _paymentService = new PaymentService(
            _mockPaymentRepository.Object,
            _mockCreditTransactionRepository.Object,
            _mockUserRepository.Object,
            _mockLogger.Object,
            _mockPaymentGateway.Object,
            _mockFrontendConfig.Object
        );
    }

    #region ProcessCreditPaymentAsync Tests

    [Fact]
    public async Task ProcessCreditPaymentAsync_WithSufficientCredit_ShouldProcessPayment()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var amount = 100.00m;
        var description = "Course enrollment payment";

        var user = UserBuilder.Create()
            .WithId(userId)
            .WithCredits(150) // Sufficient credit
            .Build();

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                          .ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                          .Returns(Task.CompletedTask);
        _mockUserRepository.Setup(x => x.SaveChangesAsync())
                          .ReturnsAsync(1);
        _mockPaymentRepository.Setup(x => x.InsertAsync(It.IsAny<Domain.Entities.Payment>()))
                             .Returns(Task.FromResult(It.IsAny<Domain.Entities.Payment>()));
        _mockPaymentRepository.Setup(x => x.SaveChangesAsync())
                             .ReturnsAsync(1);
        _mockCreditTransactionRepository.Setup(x => x.InsertAsync(It.IsAny<CreditTransaction>()))
                                       .Returns(Task.FromResult(It.IsAny<CreditTransaction>()));

        // Act
        var result = await _paymentService.ProcessCreditPaymentAsync(userId, courseId, amount, description);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeTrue();
        result.RemainingCredit.Should().Be(50); // 150 - 100 = 50
        result.Message.Should().Be("Payment processed successfully");
        result.PaymentId.Should().NotBeEmpty();
        result.TransactionId.Should().NotBeNullOrEmpty();

        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        _mockUserRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        _mockPaymentRepository.Verify(x => x.InsertAsync(It.IsAny<Domain.Entities.Payment>()), Times.Once);
        _mockPaymentRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        _mockCreditTransactionRepository.Verify(x => x.InsertAsync(It.IsAny<CreditTransaction>()), Times.Once);
    }

    [Fact]
    public async Task ProcessCreditPaymentAsync_WithInsufficientCredit_ShouldThrowException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var courseId = Guid.NewGuid();
        var amount = 150.00m;
        var description = "Course enrollment payment";

        var user = UserBuilder.Create()
            .WithId(userId)
            .WithCredits(50) // Insufficient credit
            .Build();

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                          .ReturnsAsync(user);

        // Act
        var result = await _paymentService.ProcessCreditPaymentAsync(userId, courseId, amount, description);

        // Assert
        result.Should().NotBeNull();
        result.IsSuccess.Should().BeFalse();
        result.Message.Should().Contain("Insufficient credit");
        result.RemainingCredit.Should().Be(50);

        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
        _mockPaymentRepository.Verify(x => x.InsertAsync(It.IsAny<Domain.Entities.Payment>()), Times.Never);
    }

        [Fact]
        public async Task ProcessCreditPaymentAsync_WithInvalidUser_ShouldThrowNotFoundException()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var courseId = Guid.NewGuid();
            var amount = 100m;
            var description = "Test payment";

            _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ApiException>(() =>
                _paymentService.ProcessCreditPaymentAsync(userId, courseId, amount, description));

            exception.ErrorCode.Should().Be("RESOURCE_NOT_FOUND");
            exception.StatusCode.Should().Be(HttpStatusCode.NotFound);
            exception.Message.Should().Contain("USER_NOT_FOUND");
        }

        #endregion

    #region HasSufficientCreditAsync Tests

    [Fact]
    public async Task HasSufficientCreditAsync_WithSufficientCredit_ShouldReturnTrue()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requiredAmount = 100.00m;

        var user = UserBuilder.Create()
            .WithId(userId)
            .WithCredits(150) // Sufficient credit
            .Build();

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                          .ReturnsAsync(user);

        // Act
        var result = await _paymentService.HasSufficientCreditAsync(userId, requiredAmount);

        // Assert
        result.Should().BeTrue();

        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task HasSufficientCreditAsync_WithInsufficientCredit_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requiredAmount = 150.00m;

        var user = UserBuilder.Create()
            .WithId(userId)
            .WithCredits(100) // Insufficient credit
            .Build();

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                          .ReturnsAsync(user);

        // Act
        var result = await _paymentService.HasSufficientCreditAsync(userId, requiredAmount);

        // Assert
        result.Should().BeFalse();

        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task HasSufficientCreditAsync_WithNonExistentUser_ShouldReturnFalse()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var requiredAmount = 100.00m;

        _mockUserRepository.Setup(x => x.GetByIdAsync(userId))
                          .ReturnsAsync((User?)null);

        // Act
        var result = await _paymentService.HasSufficientCreditAsync(userId, requiredAmount);

        // Assert
        result.Should().BeFalse();

        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    #endregion

    #region GetUserPaymentHistoryAsync Tests

    [Fact]
    public async Task GetUserPaymentHistoryAsync_WithValidUserId_ShouldReturnHistory()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        var payments = new List<Domain.Entities.Payment>
        {
            PaymentBuilder.Create()
                .WithUserId(userId)
                .WithAmount(100.00m)
                .WithDescription("Course payment 1")
                .Build(),
            PaymentBuilder.Create()
                .WithUserId(userId)
                .WithAmount(50.00m)
                .WithDescription("Course payment 2")
                .Build()
        };

        _mockPaymentRepository.Setup(x => x.GetUserPaymentsAsync(userId))
                             .ReturnsAsync(payments);

        // Act
        var result = await _paymentService.GetUserPaymentHistoryAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().HaveCount(2);
        
        var resultList = result.ToList();
        resultList[0].UserId.Should().Be(userId);
        resultList[0].Amount.Should().Be(100.00m);
        resultList[0].Description.Should().Be("Course payment 1");
        
        resultList[1].UserId.Should().Be(userId);
        resultList[1].Amount.Should().Be(50.00m);
        resultList[1].Description.Should().Be("Course payment 2");

        _mockPaymentRepository.Verify(x => x.GetUserPaymentsAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GetUserPaymentHistoryAsync_WithNoPayments_ShouldReturnEmptyList()
    {
        // Arrange
        var userId = Guid.NewGuid();
        
        _mockPaymentRepository.Setup(x => x.GetUserPaymentsAsync(userId))
                             .ReturnsAsync(new List<Domain.Entities.Payment>());

        // Act
        var result = await _paymentService.GetUserPaymentHistoryAsync(userId);

        // Assert
        result.Should().NotBeNull();
        result.Should().BeEmpty();

        _mockPaymentRepository.Verify(x => x.GetUserPaymentsAsync(userId), Times.Once);
    }

    #endregion

    #region CreatePaymentLinkAsync Tests

    [Fact]
    public async Task CreateCreditPurchaseAsync_WithValidData_ShouldReturnPaymentLink()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var creditAmount = 100;
        var user = new UserBuilder()
            .WithId(userId)
            .WithName("Test User")
            .Build();

        var createPaymentResult = new CreatePaymentResultDto
        {
            CheckoutUrl = "https://pay.payos.vn/web/12345",
            OrderCode = 12345,
            Amount = 100,
            Description = "Nap 100 credit"
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(userId))
            .ReturnsAsync(user);
        _mockPaymentGateway.Setup(x => x.CreatePaymentLinkAsync(It.IsAny<PaymentDataDto>()))
                          .ReturnsAsync(createPaymentResult);
        _mockPaymentRepository.Setup(r => r.InsertAsync(It.IsAny<Domain.Entities.Payment>()))
            .ReturnsAsync((Domain.Entities.Payment p) => p);
        _mockPaymentRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _paymentService.CreateCreditPurchaseAsync(userId, creditAmount);

        // Assert
        result.Should().NotBeNull();
        result.PaymentUrl.Should().Be("https://pay.payos.vn/web/12345");
        result.OrderCode.Should().BeGreaterThan(0); // Generated from timestamp
        result.Amount.Should().Be(100);
        result.Description.Should().Be("Nap 100 credit");

        _mockPaymentGateway.Verify(x => x.CreatePaymentLinkAsync(It.IsAny<PaymentDataDto>()), Times.Once);
        _mockPaymentRepository.Verify(r => r.InsertAsync(It.Is<Domain.Entities.Payment>(p => 
            p.userId == userId && 
            p.amount == creditAmount && 
            p.status == PaymentStatus.Pending &&
            p.PaymentType == PaymentType.Money)), Times.Once);
    }

    #endregion

    #region ProcessPaymentCallbackAsync Tests

    [Fact]
    public async Task ProcessPayOSWebhookAsync_WithValidCallback_ShouldUpdatePaymentStatus()
    {
        // Arrange
        var webhookBody = new WebhookTypeDto
        {
            Code = "00",
            Desc = "Success",
            Success = true,
            Data = new WebhookDataDto
            {
                OrderCode = 12345,
                Amount = 100000,
                Description = "Nap 100 credit",
                TransactionDateTime = DateTime.UtcNow.ToString(),
                Reference = "REF123",
                Code = "00",
                Desc = "Success"
            },
            Signature = "valid_signature"
        };

        var verifiedData = new WebhookDataDto
        {
            OrderCode = 12345,
            Amount = 100000,
            Description = "Nap 100 credit",
            Code = "00",
            Desc = "Success"
        };

        _mockPaymentGateway.Setup(x => x.VerifyPaymentWebhookData(webhookBody))
                          .Returns(verifiedData);

        var existingPayment = PaymentBuilder.Create()
            .WithTransactionId("12345")
            .AsPending()
            .WithAmount(100000)
            .Build();

        _mockPaymentRepository.Setup(x => x.GetByTransactionIdAsync("12345"))
                             .ReturnsAsync(existingPayment);
        _mockPaymentRepository.Setup(x => x.UpdateAsync(It.IsAny<Domain.Entities.Payment>()))
                             .Returns(Task.CompletedTask);
        _mockPaymentRepository.Setup(x => x.SaveChangesAsync())
                             .ReturnsAsync(1);

        var user = UserBuilder.Create()
            .WithId(existingPayment.userId)
            .WithCredits(50)
            .Build();

        _mockUserRepository.Setup(x => x.GetByIdAsync(existingPayment.userId))
                          .ReturnsAsync(user);
        _mockUserRepository.Setup(x => x.UpdateAsync(It.IsAny<User>()))
                          .Returns(Task.CompletedTask);
        _mockUserRepository.Setup(x => x.SaveChangesAsync())
                          .ReturnsAsync(1);

        _mockCreditTransactionRepository.Setup(x => x.InsertAsync(It.IsAny<CreditTransaction>()))
                                       .Returns(Task.FromResult(It.IsAny<CreditTransaction>()));

        // Act
        await _paymentService.ProcessPayOSWebhookAsync(webhookBody);

        // Assert
        _mockPaymentGateway.Verify(x => x.VerifyPaymentWebhookData(webhookBody), Times.Once);
        _mockPaymentRepository.Verify(x => x.GetByTransactionIdAsync("12345"), Times.Once);
        _mockPaymentRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Payment>()), Times.Once);
        _mockPaymentRepository.Verify(x => x.SaveChangesAsync(), Times.Once);
        _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
        _mockCreditTransactionRepository.Verify(x => x.InsertAsync(It.IsAny<CreditTransaction>()), Times.Once);
    }

    [Fact]
    public async Task ProcessPayOSWebhookAsync_WithAlreadyProcessedPayment_ShouldSkipProcessing()
    {
        // Arrange
        var webhookBody = new WebhookTypeDto
        {
            Code = "00",
            Desc = "Success",
            Success = true,
            Data = new WebhookDataDto
            {
                OrderCode = 12345,
                Amount = 100000,
                Description = "Nap 100 credit",
                Code = "00"
            },
            Signature = "valid_signature"
        };

        var verifiedData = new WebhookDataDto
        {
            OrderCode = 12345,
            Amount = 100000,
            Description = "Nap 100 credit",
            Code = "00"
        };

        _mockPaymentGateway.Setup(x => x.VerifyPaymentWebhookData(webhookBody))
                          .Returns(verifiedData);

        var existingPayment = PaymentBuilder.Create()
            .WithTransactionId("12345")
            .AsCompleted() // Already processed
            .Build();

        _mockPaymentRepository.Setup(x => x.GetByTransactionIdAsync("12345"))
                             .ReturnsAsync(existingPayment);

        // Act
        await _paymentService.ProcessPayOSWebhookAsync(webhookBody);

        // Assert
        _mockPaymentGateway.Verify(x => x.VerifyPaymentWebhookData(webhookBody), Times.Once);
        _mockPaymentRepository.Verify(x => x.GetByTransactionIdAsync("12345"), Times.Once);
        _mockPaymentRepository.Verify(x => x.UpdateAsync(It.IsAny<Domain.Entities.Payment>()), Times.Never);
        _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Never);
        _mockCreditTransactionRepository.Verify(x => x.InsertAsync(It.IsAny<CreditTransaction>()), Times.Never);
    }

    #endregion
}
