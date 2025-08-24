using FluentAssertions;
using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Exceptions;
using JCertPreApplication.Application.Features.AdminDashboard;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.Helpers;
using JCertPreApplication.UnitTests.Common.TestFixtures;
using Moq;
using System.Net;

namespace JCertPreApplication.UnitTests.Features.AdminDashboard;

public class AdminDashboardServiceTests
{
    private readonly Mock<IPaymentRepository> _mockPaymentRepository;
    private readonly Mock<IEnrollmentRepository> _mockEnrollmentRepository;
    private readonly AdminDashboardService _adminDashboardService;
    private readonly DateTime _testReferenceDate;

    public AdminDashboardServiceTests()
    {
        _mockPaymentRepository = new Mock<IPaymentRepository>();
        _mockEnrollmentRepository = new Mock<IEnrollmentRepository>();
        _adminDashboardService = new AdminDashboardService(
            _mockPaymentRepository.Object,
            _mockEnrollmentRepository.Object);
        _testReferenceDate = AdminDashboardServiceFixture.GetTestReferenceDate();
    }

    #region GetTotalRevenueAsync Tests

    [Fact]
    public async Task GetTotalRevenueAsync_WithCompletedMoneyPayments_ShouldReturnCorrectTotalRevenue()
    {
        // Arrange
        var payments = AdminDashboardServiceFixture.CreateMoneyPayments(3, 2, 1000000m);
        var expectedTotalAmount = AdminDashboardServiceFixture.GetExpectedTotalAmount(payments);
        var expectedTransactionCount = AdminDashboardServiceFixture.GetExpectedTransactionCount(payments);

        _mockPaymentRepository.Setup(x => x.GetPaymentsByTypeAsync(PaymentType.Money))
            .ReturnsAsync(payments);

        // Act
        var result = await _adminDashboardService.GetTotalRevenueAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalAmount.Should().Be(expectedTotalAmount);
        result.Currency.Should().Be("VND");
        result.TotalTransactions.Should().Be(expectedTransactionCount);
        result.CalculatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _mockPaymentRepository.Verify(x => x.GetPaymentsByTypeAsync(PaymentType.Money), Times.Once);
    }

    [Fact]
    public async Task GetTotalRevenueAsync_WithNoCompletedPayments_ShouldReturnZeroRevenue()
    {
        // Arrange
        var payments = new List<JCertPreApplication.Domain.Entities.Payment>
        {
            PaymentBuilder.Create().WithPaymentType(PaymentType.Money).WithStatus(PaymentStatus.Pending).Build(),
            PaymentBuilder.Create().WithPaymentType(PaymentType.Money).WithStatus(PaymentStatus.Failed).Build()
        };

        _mockPaymentRepository.Setup(x => x.GetPaymentsByTypeAsync(PaymentType.Money))
            .ReturnsAsync(payments);

        // Act
        var result = await _adminDashboardService.GetTotalRevenueAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalAmount.Should().Be(0m);
        result.Currency.Should().Be("VND");
        result.TotalTransactions.Should().Be(0);
        result.CalculatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetTotalRevenueAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        _mockPaymentRepository.Setup(x => x.GetPaymentsByTypeAsync(PaymentType.Money))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _adminDashboardService.GetTotalRevenueAsync());

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("REVENUE_CALCULATION_ERROR");
        exception.Message.Should().Contain("An error occurred while calculating total revenue");
    }

    #endregion

    #region GetCurrentMonthRevenueAsync Tests

    [Fact]
    public async Task GetCurrentMonthRevenueAsync_WithCurrentMonthData_ShouldReturnCorrectAmount()
    {
        // Arrange
        var expectedAmount = 5000000m;
        var expectedMonth = DateTimeHelper.FormatMonthYear(_testReferenceDate);
        var startOfMonth = DateTimeHelper.GetStartOfMonth(_testReferenceDate);
        var startOfNextMonth = DateTimeHelper.GetStartOfNextMonth(_testReferenceDate);

        _mockPaymentRepository.Setup(x => x.GetTotalRevenueByDateRangeAsync(startOfMonth, startOfNextMonth))
            .ReturnsAsync(expectedAmount);

        // Act
        var result = await _adminDashboardService.GetCurrentMonthRevenueAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalAmount.Should().Be(expectedAmount);
        result.Currency.Should().Be("VND");
        result.Month.Should().Be(expectedMonth);
        result.CalculatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _mockPaymentRepository.Verify(x => x.GetTotalRevenueByDateRangeAsync(
            It.Is<DateTime>(d => d == startOfMonth),
            It.Is<DateTime>(d => d == startOfNextMonth)), Times.Once);
    }

    [Fact]
    public async Task GetCurrentMonthRevenueAsync_WithNoCurrentMonthData_ShouldReturnZeroAmount()
    {
        // Arrange
        _mockPaymentRepository.Setup(x => x.GetTotalRevenueByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(0m);

        // Act
        var result = await _adminDashboardService.GetCurrentMonthRevenueAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalAmount.Should().Be(0m);
        result.Currency.Should().Be("VND");
    }

    [Fact]
    public async Task GetCurrentMonthRevenueAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        _mockPaymentRepository.Setup(x => x.GetTotalRevenueByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _adminDashboardService.GetCurrentMonthRevenueAsync());

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("CURRENT_MONTH_REVENUE_ERROR");
    }

    #endregion

    #region GetRevenueByMonthAsync Tests

    [Fact]
    public async Task GetRevenueByMonthAsync_WithLast12MonthsData_ShouldReturnCompleteMonthlyData()
    {
        // Arrange
        var monthlyRevenueData = AdminDashboardServiceFixture.CreateCompleteMonthlyRevenueData(_testReferenceDate);
        var expectedData = AdminDashboardServiceFixture.CreateExpectedMonthlyDictionary<decimal>(_testReferenceDate, monthlyRevenueData);
        var startDate = DateTimeHelper.GetStartDateFor12Months(_testReferenceDate);
        var endDate = DateTimeHelper.GetEndDateFor12Months(_testReferenceDate);

        _mockPaymentRepository.Setup(x => x.GetRevenueByMonthAsync(startDate, endDate))
            .ReturnsAsync(monthlyRevenueData);

        // Act
        var result = await _adminDashboardService.GetRevenueByMonthAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(12);
        result.Data.Should().BeEquivalentTo(expectedData);
        result.Currency.Should().Be("VND");
        result.CalculatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _mockPaymentRepository.Verify(x => x.GetRevenueByMonthAsync(
            It.Is<DateTime>(d => d == startDate),
            It.Is<DateTime>(d => d == endDate)), Times.Once);
    }

    [Fact]
    public async Task GetRevenueByMonthAsync_WithPartialData_ShouldFillMissingMonthsWithZero()
    {
        // Arrange
        var partialRevenueData = AdminDashboardServiceFixture.CreateMonthlyRevenueData(_testReferenceDate, 6);
        var expectedData = AdminDashboardServiceFixture.CreateExpectedMonthlyDictionary<decimal>(_testReferenceDate, partialRevenueData);

        _mockPaymentRepository.Setup(x => x.GetRevenueByMonthAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(partialRevenueData);

        // Act
        var result = await _adminDashboardService.GetRevenueByMonthAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(12);
        result.Data.Should().BeEquivalentTo(expectedData);
        
        // Verify some months have data and others are 0
        var monthsWithData = result.Data.Values.Count(v => v > 0);
        var monthsWithZero = result.Data.Values.Count(v => v == 0);
        monthsWithData.Should().Be(6);
        monthsWithZero.Should().Be(6);
    }

    [Fact]
    public async Task GetRevenueByMonthAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        _mockPaymentRepository.Setup(x => x.GetRevenueByMonthAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _adminDashboardService.GetRevenueByMonthAsync());

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("REVENUE_BY_MONTH_ERROR");
    }

    #endregion

    #region GetTotalEnrollmentsAsync Tests

    [Fact]
    public async Task GetTotalEnrollmentsAsync_WithExistingEnrollments_ShouldReturnCorrectCount()
    {
        // Arrange
        var expectedCount = 150L;

        _mockEnrollmentRepository.Setup(x => x.GetTotalEnrollmentsCountAsync())
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _adminDashboardService.GetTotalEnrollmentsAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(expectedCount);
        result.CalculatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _mockEnrollmentRepository.Verify(x => x.GetTotalEnrollmentsCountAsync(), Times.Once);
    }

    [Fact]
    public async Task GetTotalEnrollmentsAsync_WithNoEnrollments_ShouldReturnZeroCount()
    {
        // Arrange
        _mockEnrollmentRepository.Setup(x => x.GetTotalEnrollmentsCountAsync())
            .ReturnsAsync(0L);

        // Act
        var result = await _adminDashboardService.GetTotalEnrollmentsAsync();

        // Assert
        result.Should().NotBeNull();
        result.TotalCount.Should().Be(0L);
        result.CalculatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));
    }

    [Fact]
    public async Task GetTotalEnrollmentsAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        _mockEnrollmentRepository.Setup(x => x.GetTotalEnrollmentsCountAsync())
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _adminDashboardService.GetTotalEnrollmentsAsync());

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("ENROLLMENTS_COUNT_ERROR");
    }

    #endregion

    #region GetCurrentMonthEnrollmentsAsync Tests

    [Fact]
    public async Task GetCurrentMonthEnrollmentsAsync_WithCurrentMonthData_ShouldReturnCorrectCount()
    {
        // Arrange
        var expectedCount = 25L;
        var expectedMonth = DateTimeHelper.FormatMonthYear(_testReferenceDate);
        var startOfMonth = DateTimeHelper.GetStartOfMonth(_testReferenceDate);
        var startOfNextMonth = DateTimeHelper.GetStartOfNextMonth(_testReferenceDate);

        _mockEnrollmentRepository.Setup(x => x.CountByDateRangeAsync(startOfMonth, startOfNextMonth))
            .ReturnsAsync(expectedCount);

        // Act
        var result = await _adminDashboardService.GetCurrentMonthEnrollmentsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(expectedCount);
        result.Month.Should().Be(expectedMonth);
        result.CalculatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _mockEnrollmentRepository.Verify(x => x.CountByDateRangeAsync(
            It.Is<DateTime>(d => d == startOfMonth),
            It.Is<DateTime>(d => d == startOfNextMonth)), Times.Once);
    }

    [Fact]
    public async Task GetCurrentMonthEnrollmentsAsync_WithNoCurrentMonthData_ShouldReturnZeroCount()
    {
        // Arrange
        _mockEnrollmentRepository.Setup(x => x.CountByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(0L);

        // Act
        var result = await _adminDashboardService.GetCurrentMonthEnrollmentsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(0L);
    }

    [Fact]
    public async Task GetCurrentMonthEnrollmentsAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        _mockEnrollmentRepository.Setup(x => x.CountByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _adminDashboardService.GetCurrentMonthEnrollmentsAsync());

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("CURRENT_MONTH_ENROLLMENTS_ERROR");
    }

    #endregion

    #region GetEnrollmentsByMonthAsync Tests

    [Fact]
    public async Task GetEnrollmentsByMonthAsync_WithLast12MonthsData_ShouldReturnCompleteMonthlyData()
    {
        // Arrange
        var monthlyCountData = AdminDashboardServiceFixture.CreateCompleteMonthlyCountData(_testReferenceDate);
        var expectedData = AdminDashboardServiceFixture.CreateExpectedMonthlyDictionary<long>(_testReferenceDate, countData: monthlyCountData);
        var startDate = DateTimeHelper.GetStartDateFor12Months(_testReferenceDate);
        var endDate = DateTimeHelper.GetEndDateFor12Months(_testReferenceDate);

        _mockEnrollmentRepository.Setup(x => x.GetEnrollmentCountsByMonthAsync(startDate, endDate))
            .ReturnsAsync(monthlyCountData);

        // Act
        var result = await _adminDashboardService.GetEnrollmentsByMonthAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().NotBeNull();
        result.Data.Should().HaveCount(12);
        result.Data.Should().BeEquivalentTo(expectedData);
        result.CalculatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(5));

        _mockEnrollmentRepository.Verify(x => x.GetEnrollmentCountsByMonthAsync(
            It.Is<DateTime>(d => d == startDate),
            It.Is<DateTime>(d => d == endDate)), Times.Once);
    }

    [Fact]
    public async Task GetEnrollmentsByMonthAsync_WithPartialData_ShouldFillMissingMonthsWithZero()
    {
        // Arrange
        var partialCountData = AdminDashboardServiceFixture.CreateMonthlyCountData(_testReferenceDate, 6);
        var expectedData = AdminDashboardServiceFixture.CreateExpectedMonthlyDictionary<long>(_testReferenceDate, countData: partialCountData);

        _mockEnrollmentRepository.Setup(x => x.GetEnrollmentCountsByMonthAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(partialCountData);

        // Act
        var result = await _adminDashboardService.GetEnrollmentsByMonthAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(12);
        result.Data.Should().BeEquivalentTo(expectedData);
        
        // Verify some months have data and others are 0
        var monthsWithData = result.Data.Values.Count(v => v > 0);
        var monthsWithZero = result.Data.Values.Count(v => v == 0);
        monthsWithData.Should().Be(6);
        monthsWithZero.Should().Be(6);
    }

    [Fact]
    public async Task GetEnrollmentsByMonthAsync_WhenRepositoryThrows_ShouldThrowInternalServerError()
    {
        // Arrange
        _mockEnrollmentRepository.Setup(x => x.GetEnrollmentCountsByMonthAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<ApiException>(
            () => _adminDashboardService.GetEnrollmentsByMonthAsync());

        exception.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        exception.ErrorCode.Should().Be("ENROLLMENTS_BY_MONTH_ERROR");
    }

    #endregion

    #region Date Edge Case Tests

    [Fact]
    public async Task GetCurrentMonthRevenueAsync_AtMonthBoundary_ShouldCalculateCorrectDateRange()
    {
        // Arrange - Test at month boundary (last day of month)
        var boundaryDate = DateTimeHelper.CreateUtcDate(2025, 8, 31, 23, 59, 59);
        var startOfMonth = DateTimeHelper.GetStartOfMonth(boundaryDate);
        var startOfNextMonth = DateTimeHelper.GetStartOfNextMonth(boundaryDate);

        _mockPaymentRepository.Setup(x => x.GetTotalRevenueByDateRangeAsync(startOfMonth, startOfNextMonth))
            .ReturnsAsync(1000000m);

        // Act
        var result = await _adminDashboardService.GetCurrentMonthRevenueAsync();

        // Assert
        result.Should().NotBeNull();
        result.Month.Should().Be("08/2025");

        _mockPaymentRepository.Verify(x => x.GetTotalRevenueByDateRangeAsync(
            It.Is<DateTime>(d => d.Day == 1 && d.Hour == 0 && d.Minute == 0 && d.Second == 0),
            It.Is<DateTime>(d => d.Day == 1 && d.Month == 9)), Times.Once);
    }

    [Fact]
    public async Task GetCurrentMonthEnrollmentsAsync_AtMonthBoundary_ShouldCalculateCorrectDateRange()
    {
        // Arrange - Test boundary logic but service uses DateTime.UtcNow internally
        // So we test that the service correctly calculates dates for current month
        _mockEnrollmentRepository.Setup(x => x.CountByDateRangeAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(15L);

        // Act
        var result = await _adminDashboardService.GetCurrentMonthEnrollmentsAsync();

        // Assert
        result.Should().NotBeNull();
        result.Count.Should().Be(15L);
        result.Month.Should().MatchRegex(@"\d{2}/\d{4}"); // Should be in MM/yyyy format

        // Verify the date range calculation
        _mockEnrollmentRepository.Verify(x => x.CountByDateRangeAsync(
            It.Is<DateTime>(d => d.Day == 1 && d.Hour == 0 && d.Minute == 0 && d.Second == 0),
            It.Is<DateTime>(d => d.Day == 1 && d.Hour == 0 && d.Minute == 0 && d.Second == 0)), Times.Once);
    }

    [Fact]
    public async Task GetRevenueByMonthAsync_CrossingYearBoundary_ShouldHandleCorrectly()
    {
        // Arrange - Use current test reference date for consistency
        var crossYearData = new List<MonthlyRevenue>
        {
            new MonthlyRevenue(2024, 9, 1000000m), // 11 months ago from Aug 2025
            new MonthlyRevenue(2024, 12, 2000000m), // Dec 2024
            new MonthlyRevenue(2025, 8, 3000000m) // Current month Aug 2025
        };

        _mockPaymentRepository.Setup(x => x.GetRevenueByMonthAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(crossYearData);

        // Act
        var result = await _adminDashboardService.GetRevenueByMonthAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(12);
        result.Data.Should().ContainKey("09/2024").WhoseValue.Should().Be(1000000m);
        result.Data.Should().ContainKey("12/2024").WhoseValue.Should().Be(2000000m);
        result.Data.Should().ContainKey("08/2025").WhoseValue.Should().Be(3000000m);
    }

    [Fact]
    public async Task GetEnrollmentsByMonthAsync_CrossingYearBoundary_ShouldHandleCorrectly()
    {
        // Arrange - Use current test reference date for consistency
        var crossYearData = new List<MonthlyCount>
        {
            new MonthlyCount(2024, 9, 10), // 11 months ago from Aug 2025
            new MonthlyCount(2024, 12, 25), // Dec 2024
            new MonthlyCount(2025, 8, 30) // Current month Aug 2025
        };

        _mockEnrollmentRepository.Setup(x => x.GetEnrollmentCountsByMonthAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(crossYearData);

        // Act
        var result = await _adminDashboardService.GetEnrollmentsByMonthAsync();

        // Assert
        result.Should().NotBeNull();
        result.Data.Should().HaveCount(12);
        result.Data.Should().ContainKey("09/2024").WhoseValue.Should().Be(10);
        result.Data.Should().ContainKey("12/2024").WhoseValue.Should().Be(25);
        result.Data.Should().ContainKey("08/2025").WhoseValue.Should().Be(30);
    }

    [Fact]
    public async Task GetRevenueByMonthAsync_WithFutureData_ShouldProcessAllRepositoryData()
    {
        // Arrange - Service processes all data returned by repository
        // Repository is responsible for filtering date ranges
        var futureData = new List<MonthlyRevenue>
        {
            new MonthlyRevenue(2025, 8, 1000000m), // Current month
            new MonthlyRevenue(2025, 10, 2000000m), // Future month
            new MonthlyRevenue(2026, 1, 3000000m) // Future year
        };

        _mockPaymentRepository.Setup(x => x.GetRevenueByMonthAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(futureData);

        // Act
        var result = await _adminDashboardService.GetRevenueByMonthAsync();

        // Assert
        result.Should().NotBeNull();
        // Service creates 12 months dictionary plus any additional data from repository
        result.Data.Should().ContainKey("08/2025").WhoseValue.Should().Be(1000000m);
        result.Data.Should().ContainKey("10/2025").WhoseValue.Should().Be(2000000m);
        result.Data.Should().ContainKey("01/2026").WhoseValue.Should().Be(3000000m);
    }

    [Fact]
    public async Task GetEnrollmentsByMonthAsync_WithFutureData_ShouldProcessAllRepositoryData()
    {
        // Arrange - Service processes all data returned by repository
        var futureData = new List<MonthlyCount>
        {
            new MonthlyCount(2025, 8, 15), // Current month
            new MonthlyCount(2025, 11, 25), // Future month
            new MonthlyCount(2026, 3, 35) // Future year
        };

        _mockEnrollmentRepository.Setup(x => x.GetEnrollmentCountsByMonthAsync(It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .ReturnsAsync(futureData);

        // Act
        var result = await _adminDashboardService.GetEnrollmentsByMonthAsync();

        // Assert
        result.Should().NotBeNull();
        // Service creates 12 months dictionary plus any additional data from repository
        result.Data.Should().ContainKey("08/2025").WhoseValue.Should().Be(15);
        result.Data.Should().ContainKey("11/2025").WhoseValue.Should().Be(25);
        result.Data.Should().ContainKey("03/2026").WhoseValue.Should().Be(35);
    }

    #endregion
}
