namespace Otlob.IServices;

public interface IAdminMonthlyAnalyticsService
{
    void Add();
    void Update(decimal totalOrderPrice);
    AdminMonthlyAnalyticsResponse? GetCurrentMonthAnalytics();
    AdminMonthlyAnalyticsResponse? GetByDate(int year, int month);
    AdminMonthlyAnalyticsResponse? GetCurrentYearAnalytics();
    AdminMonthlyAnalyticsResponse? GetByYearAnalytics(int year);
    List<AdminMonthlyAnalyticsResponse> GetLastTweleveMonthsAnalytics();
}
