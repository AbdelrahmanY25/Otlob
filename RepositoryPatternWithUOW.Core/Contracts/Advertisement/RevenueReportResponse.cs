namespace Otlob.Core.Contracts.Advertisement;

public class RevenueReportResponse
{
    public decimal TotalRevenue { get; set; }
    public decimal ThisMonthRevenue { get; set; }
    public decimal ThisWeekRevenue { get; set; }
    public decimal TotalRefunds { get; set; }
    public decimal NetProfit => TotalRevenue - TotalRefunds;
    
    public int TotalAdsCount { get; set; }
    public int ActiveAdsCount { get; set; }
    public int PendingAdsCount { get; set; }
    public int RejectedAdsCount { get; set; }
    
    public List<RevenueByPlanResponse> RevenueByPlan { get; set; } = [];
    public List<TopRestaurantResponse> TopRestaurants { get; set; } = [];
    
    public string TotalRevenueDisplay => $"{TotalRevenue:N0} EGP";
    public string ThisMonthRevenueDisplay => $"{ThisMonthRevenue:N0} EGP";
    public string ThisWeekRevenueDisplay => $"{ThisWeekRevenue:N0} EGP";
    public string TotalRefundsDisplay => $"{TotalRefunds:N0} EGP";
    public string NetProfitDisplay => $"{NetProfit:N0} EGP";
}

public class RevenueByPlanResponse
{
    public int PlanId { get; set; }
    public string PlanName { get; set; } = string.Empty;
    public int AdsCount { get; set; }
    public decimal Revenue { get; set; }
    public decimal PercentageOfTotal { get; set; }
    
    public string RevenueDisplay => $"{Revenue:N0} EGP";
    public string PercentageDisplay => $"{PercentageOfTotal:N1}%";
}

public class TopRestaurantResponse
{
    public int RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string? RestaurantImage { get; set; }
    public decimal TotalPaid { get; set; }
    public int ActiveAdsCount { get; set; }
    public int TotalAdsCount { get; set; }
    
    public string TotalPaidDisplay => $"{TotalPaid:N0} EGP";
}
