namespace Otlob.Core.Contracts.AdminAnalytics;

public class SuperAdminGeneralAnalyticsResponse
{
    public int TotalOrdersCount { get; init; }
    public decimal AverageOrdersPerDay { get; init; }
    public int TotalCancelledOrdersCount { get; init; }
    public decimal TotalSales { get; init; }
    public decimal TotalRevenue { get; init; }
    public List<AdminMonthlyAnalyticsResponse> AllMonthsAnalytics { get; init; } = [];
}
