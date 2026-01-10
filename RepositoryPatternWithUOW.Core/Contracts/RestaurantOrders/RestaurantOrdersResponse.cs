namespace Otlob.Core.Contracts.RestaurantOrders;

public class RestaurantOrdersResponse
{
    public int Id { get; init; }
    public string UserId { get; init; } = string.Empty;
    public string RestaurantId { get; init; } = string.Empty;
    public DateTime OrderDate { get; init; }
    public decimal TotalAmount { get; init; }
    public string Status { get; init; } = string.Empty;
    public PaymentMethod PaymentMethod { get; init; }
    public int ItemsCount { get; init; }
}
