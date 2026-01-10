namespace Otlob.Core.Contracts.AdminAnalytics;

public class SuperAdminDashboardResponse
{
    public AdminDailyAnalyticsResponse? TodayAnalytics { get; init; }
    public AdminMonthlyAnalyticsResponse? CurrentMonthAnalytics { get; init; }
    public AdminMonthlyAnalyticsResponse? CurrentYearAnalytics { get; init; }
    public List<AdminMonthlyAnalyticsResponse> Last12MonthsAnalytics { get; init; } = [];
}