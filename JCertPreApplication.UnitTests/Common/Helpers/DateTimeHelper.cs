using System;
using System.Collections.Generic;

namespace JCertPreApplication.UnitTests.Common.Helpers;

public static class DateTimeHelper
{
    /// <summary>
    /// Get start of month for given date
    /// </summary>
    public static DateTime GetStartOfMonth(DateTime date)
    {
        return DateTime.SpecifyKind(new DateTime(date.Year, date.Month, 1), DateTimeKind.Utc);
    }

    /// <summary>
    /// Get end of month for given date
    /// </summary>
    public static DateTime GetEndOfMonth(DateTime date)
    {
        var startOfMonth = GetStartOfMonth(date);
        return startOfMonth.AddMonths(1).AddTicks(-1);
    }

    /// <summary>
    /// Get start of next month for given date
    /// </summary>
    public static DateTime GetStartOfNextMonth(DateTime date)
    {
        var startOfMonth = GetStartOfMonth(date);
        return startOfMonth.AddMonths(1);
    }

    /// <summary>
    /// Get start date for last 12 months calculation (11 months ago from current month start)
    /// </summary>
    public static DateTime GetStartDateFor12Months(DateTime referenceDate)
    {
        var startOfCurrentMonth = GetStartOfMonth(referenceDate);
        return startOfCurrentMonth.AddMonths(-11);
    }

    /// <summary>
    /// Get end date for last 12 months calculation (start of next month)
    /// </summary>
    public static DateTime GetEndDateFor12Months(DateTime referenceDate)
    {
        return GetStartOfNextMonth(referenceDate);
    }

    /// <summary>
    /// Format date to "MM/yyyy" string
    /// </summary>
    public static string FormatMonthYear(DateTime date)
    {
        return date.ToString("MM/yyyy");
    }

    /// <summary>
    /// Get list of start dates for last 12 months
    /// </summary>
    public static List<DateTime> GetLast12MonthsStartDates(DateTime referenceDate)
    {
        var dates = new List<DateTime>();
        var startDate = GetStartDateFor12Months(referenceDate);
        
        for (int i = 0; i < 12; i++)
        {
            dates.Add(startDate.AddMonths(i));
        }
        
        return dates;
    }

    /// <summary>
    /// Get list of month-year strings for last 12 months
    /// </summary>
    public static List<string> GetLast12MonthsKeys(DateTime referenceDate)
    {
        var keys = new List<string>();
        var startDates = GetLast12MonthsStartDates(referenceDate);
        
        foreach (var date in startDates)
        {
            keys.Add(FormatMonthYear(date));
        }
        
        return keys;
    }

    /// <summary>
    /// Create test date with UTC kind
    /// </summary>
    public static DateTime CreateUtcDate(int year, int month, int day, int hour = 0, int minute = 0, int second = 0)
    {
        return DateTime.SpecifyKind(new DateTime(year, month, day, hour, minute, second), DateTimeKind.Utc);
    }
}
