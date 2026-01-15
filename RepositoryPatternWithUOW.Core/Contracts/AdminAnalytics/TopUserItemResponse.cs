namespace Otlob.Core.Contracts.AdminAnalytics;

public class TopUserItemResponse
{
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string? UserImage { get; init; }
    public int OrdersCount { get; init; }
    public decimal TotalSpent { get; init; }
}
