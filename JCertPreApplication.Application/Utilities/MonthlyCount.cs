namespace JCertPreApplication.Application.Utilities
{
    /// <summary>
    /// Record to represent monthly enrollment count data
    /// </summary>
    /// <param name="Year">Year value</param>
    /// <param name="Month">Month value (1-12)</param>
    /// <param name="Count">Number of enrollments in that month</param>
    public record MonthlyCount(int Year, int Month, long Count);
}
