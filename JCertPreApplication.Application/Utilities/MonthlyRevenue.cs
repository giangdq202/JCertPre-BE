namespace JCertPreApplication.Application.Utilities
{
    /// <summary>
    /// Record to represent monthly revenue data
    /// </summary>
    /// <param name="Year">Year value</param>
    /// <param name="Month">Month value (1-12)</param>
    /// <param name="TotalAmount">Total revenue amount in that month</param>
    public record MonthlyRevenue(int Year, int Month, decimal TotalAmount);
}
