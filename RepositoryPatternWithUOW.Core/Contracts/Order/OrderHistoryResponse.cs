namespace Otlob.Core.Contracts.OrderDetails;

public class OrderHistoryResponse
{
    public int Id { get; set; }
    public string RestaurantName { get; set; } = string.Empty;
    public string? RestaurantImage { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public decimal TotalPrice { get; set; }
    public int ItemsCount { get; set; }
    public bool IsRated { get; set; }
}
