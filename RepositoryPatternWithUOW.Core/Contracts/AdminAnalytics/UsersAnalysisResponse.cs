namespace Otlob.Core.Contracts.AdminAnalytics;

public class UsersAnalysisResponse
{
    public int TotalUsers { get; init; }
    public int ActiveUsersCount { get; init; }
    public int InactiveUsersCount { get; init; }
    public decimal ActivePercentage { get; init; }
    public decimal InactivePercentage { get; init; }

    public List<TopUserItemResponse> TopUsersByOrders { get; init; } = [];

    // Optional additional analytics
    public decimal AverageOrdersPerActiveUser { get; init; }
    
    // Repeat customer metrics - most important business metric
    public int RepeatCustomersCount { get; init; }
    public decimal RepeatCustomersPercentage { get; init; }
}
