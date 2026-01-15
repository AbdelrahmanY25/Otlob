namespace Otlob.Core.Contracts.AdminAnalytics;

public class AllRestaurantsAnalyticsDashboardResponse
{
    // Summary Statistics
    public int TotalRestaurants { get; init; }
    public int TotalCompletedOrders { get; init; }
    public int TotalCancelledOrders { get; init; }
    public decimal TotalSales { get; init; }
    public decimal TotalRevenue { get; init; }
    
    // Current Period Analytics
    public decimal CurrentMonthSales { get; init; }
    public int CurrentMonthOrders { get; init; }
    public decimal CurrentYearSales { get; init; }
    public int CurrentYearOrders { get; init; }
    
    // Top Restaurants by Sales
    public List<RestaurantAnalyticsItemResponse> TopRestaurantsBySales { get; init; } = [];
    
    // Top Restaurants by Orders Count
    public List<RestaurantAnalyticsItemResponse> TopRestaurantsByOrdersCount { get; init; } = [];
    
    // Monthly Trend Data (Last 12 months aggregated from all restaurants)
    public List<MonthlyTrendData> MonthlyTrends { get; init; } = [];
    
    // All Restaurant Analytics (for detailed view)
    public List<RestaurantAnalyticsItemResponse> AllRestaurantsAnalytics { get; init; } = [];
}

public class MonthlyTrendData
{
    public int Year { get; init; }
    public int Month { get; init; }
    public int TotalOrders { get; init; }
    public decimal TotalSales { get; init; }
    public decimal TotalRevenue { get; init; }
    public int RestaurantCount { get; init; }
}
