using JCertPreApplication.Application.Contracts;
using JCertPreApplication.Application.Features.AdminDashboard;
using JCertPreApplication.Application.Utilities;
using JCertPreApplication.Domain.Entities;
using JCertPreApplication.Domain.Enums;
using JCertPreApplication.UnitTests.Common.Builders;
using JCertPreApplication.UnitTests.Common.Helpers;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace JCertPreApplication.UnitTests.Common.TestFixtures;

public class AdminDashboardServiceFixture
{
    public AdminDashboardService AdminDashboardService { get; }
    public Mock<IPaymentRepository> MockPaymentRepository { get; }
    public Mock<IEnrollmentRepository> MockEnrollmentRepository { get; }

    public AdminDashboardServiceFixture()
    {
        MockPaymentRepository = new Mock<IPaymentRepository>();
        MockEnrollmentRepository = new Mock<IEnrollmentRepository>();

        AdminDashboardService = new AdminDashboardService(
            MockPaymentRepository.Object,
            MockEnrollmentRepository.Object);
    }

    /// <summary>
    /// Create list of Money payments with specified statuses and amounts
    /// </summary>
    public static List<Payment> CreateMoneyPayments(int completedCount = 2, int pendingCount = 1, decimal baseAmount = 1000000m)
    {
        var payments = new List<Payment>();
        
        // Create completed payments
        for (int i = 0; i < completedCount; i++)
        {
            payments.Add(PaymentBuilder.Create()
                .WithPaymentType(PaymentType.Money)
                .WithStatus(PaymentStatus.Completed)
                .WithAmount(baseAmount * (i + 1))
                .WithCreatedAt(DateTimeHelper.CreateUtcDate(2025, 8, i + 1))
                .Build());
        }
        
        // Create pending payments (should be excluded from revenue)
        for (int i = 0; i < pendingCount; i++)
        {
            payments.Add(PaymentBuilder.Create()
                .WithPaymentType(PaymentType.Money)
                .WithStatus(PaymentStatus.Pending)
                .WithAmount(baseAmount * 0.5m)
                .WithCreatedAt(DateTimeHelper.CreateUtcDate(2025, 8, i + 10))
                .Build());
        }
        
        return payments;
    }

    /// <summary>
    /// Create list of Credit payments (should be excluded from revenue)
    /// </summary>
    public static List<Payment> CreateCreditPayments(int count = 2, decimal baseAmount = 500000m)
    {
        var payments = new List<Payment>();
        
        for (int i = 0; i < count; i++)
        {
            payments.Add(PaymentBuilder.Create()
                .WithPaymentType(PaymentType.Credit)
                .WithStatus(PaymentStatus.Completed)
                .WithAmount(baseAmount)
                .WithCreatedAt(DateTimeHelper.CreateUtcDate(2025, 8, i + 1))
                .Build());
        }
        
        return payments;
    }

    /// <summary>
    /// Create monthly revenue data for testing
    /// </summary>
    public static List<MonthlyRevenue> CreateMonthlyRevenueData(DateTime referenceDate, int monthsWithData = 6)
    {
        var data = new List<MonthlyRevenue>();
        var startDate = DateTimeHelper.GetStartDateFor12Months(referenceDate);
        
        for (int i = 0; i < monthsWithData; i++)
        {
            var date = startDate.AddMonths(i * 2); // Every 2 months to create gaps
            data.Add(new MonthlyRevenue(date.Year, date.Month, (i + 1) * 1500000m));
        }
        
        return data;
    }

    /// <summary>
    /// Create monthly count data for testing
    /// </summary>
    public static List<MonthlyCount> CreateMonthlyCountData(DateTime referenceDate, int monthsWithData = 6)
    {
        var data = new List<MonthlyCount>();
        var startDate = DateTimeHelper.GetStartDateFor12Months(referenceDate);
        
        for (int i = 0; i < monthsWithData; i++)
        {
            var date = startDate.AddMonths(i * 2); // Every 2 months to create gaps
            data.Add(new MonthlyCount(date.Year, date.Month, (i + 1) * 15));
        }
        
        return data;
    }

    /// <summary>
    /// Create complete monthly revenue data (all 12 months)
    /// </summary>
    public static List<MonthlyRevenue> CreateCompleteMonthlyRevenueData(DateTime referenceDate)
    {
        var data = new List<MonthlyRevenue>();
        var startDate = DateTimeHelper.GetStartDateFor12Months(referenceDate);
        
        for (int i = 0; i < 12; i++)
        {
            var date = startDate.AddMonths(i);
            data.Add(new MonthlyRevenue(date.Year, date.Month, (i + 1) * 1000000m));
        }
        
        return data;
    }

    /// <summary>
    /// Create complete monthly count data (all 12 months)
    /// </summary>
    public static List<MonthlyCount> CreateCompleteMonthlyCountData(DateTime referenceDate)
    {
        var data = new List<MonthlyCount>();
        var startDate = DateTimeHelper.GetStartDateFor12Months(referenceDate);
        
        for (int i = 0; i < 12; i++)
        {
            var date = startDate.AddMonths(i);
            data.Add(new MonthlyCount(date.Year, date.Month, (i + 1) * 10));
        }
        
        return data;
    }

    /// <summary>
    /// Create enrollments for testing
    /// </summary>
    public static List<Enrollment> CreateEnrollments(int count, DateTime? enrollmentDate = null)
    {
        var enrollments = new List<Enrollment>();
        var baseDate = enrollmentDate ?? DateTimeHelper.CreateUtcDate(2025, 8, 1);
        
        for (int i = 0; i < count; i++)
        {
            enrollments.Add(EnrollmentBuilder.Create()
                .WithEnrollDate(baseDate.AddDays(i))
                .Build());
        }
        
        return enrollments;
    }

    /// <summary>
    /// Get expected total amount from completed Money payments
    /// </summary>
    public static decimal GetExpectedTotalAmount(List<Payment> payments)
    {
        return payments
            .Where(p => p.PaymentType == PaymentType.Money && p.status == PaymentStatus.Completed)
            .Sum(p => p.amount);
    }

    /// <summary>
    /// Get expected transaction count from completed Money payments
    /// </summary>
    public static int GetExpectedTransactionCount(List<Payment> payments)
    {
        return payments
            .Count(p => p.PaymentType == PaymentType.Money && p.status == PaymentStatus.Completed);
    }

    /// <summary>
    /// Create expected monthly data dictionary with all 12 months initialized to 0
    /// </summary>
    public static Dictionary<string, T> CreateExpectedMonthlyDictionary<T>(DateTime referenceDate, List<MonthlyRevenue> revenueData = null, List<MonthlyCount> countData = null) where T : struct
    {
        var result = new Dictionary<string, T>();
        var monthKeys = DateTimeHelper.GetLast12MonthsKeys(referenceDate);
        
        // Initialize all months with default value (0)
        foreach (var key in monthKeys)
        {
            result[key] = default(T);
        }
        
        // Update with actual revenue data
        if (revenueData != null && typeof(T) == typeof(decimal))
        {
            foreach (var data in revenueData)
            {
                var key = $"{data.Month:D2}/{data.Year}";
                if (result.ContainsKey(key))
                {
                    result[key] = (T)(object)data.TotalAmount;
                }
            }
        }
        
        // Update with actual count data
        if (countData != null && typeof(T) == typeof(long))
        {
            foreach (var data in countData)
            {
                var key = $"{data.Month:D2}/{data.Year}";
                if (result.ContainsKey(key))
                {
                    result[key] = (T)(object)data.Count;
                }
            }
        }
        
        return result;
    }

    /// <summary>
    /// Create test reference date for consistent testing
    /// </summary>
    public static DateTime GetTestReferenceDate()
    {
        return DateTimeHelper.CreateUtcDate(2025, 8, 15, 10, 30, 0);
    }
}
