namespace Otlob.Core.Contracts.Advertisement;

public class RestaurantCostsResponse
{
    public int RestaurantId { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    
    public decimal TotalSpent { get; set; }
    public decimal ActiveAdsCost { get; set; }
    public decimal ThisMonthSpent { get; set; }
    public decimal TotalRefunded { get; set; }
    
    public int TotalAdsCount { get; set; }
    public int ActiveAdsCount { get; set; }
    public int PendingAdsCount { get; set; }
    
    public List<PaymentHistoryResponse> PaymentHistory { get; set; } = [];
    
    public string TotalSpentDisplay => $"{TotalSpent:N0} EGP";
    public string ActiveAdsCostDisplay => $"{ActiveAdsCost:N0} EGP";
    public string ThisMonthSpentDisplay => $"{ThisMonthSpent:N0} EGP";
    public string TotalRefundedDisplay => $"{TotalRefunded:N0} EGP";
}
