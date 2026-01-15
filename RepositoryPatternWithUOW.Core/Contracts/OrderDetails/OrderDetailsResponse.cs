namespace Otlob.Core.Contracts.OrderDetails;

public class OrderDetailsResponse
{
    public int Id { get; set; }
    public string Restaurantkey { get; set; } = string.Empty;
    public string RestaurantName { get; set; } = string.Empty;
    public string? RestaurantImage { get; set; }
    public DateTime OrderDate { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public string DeliveryAddress { get; set; } = string.Empty;
    public string CustomerPhoneNumber { get; set; } = string.Empty;
    public string? Notes { get; set; }
    
    // Pricing
    public decimal SubTotal { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal ServiceFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalPrice { get; set; }
    
    // Rating
    public bool IsRated { get; set; }
    
    // Order Items
    public List<OrderItemResponse> Items { get; set; } = [];
}

public class OrderItemResponse
{
    public string MealName { get; set; } = string.Empty;
    public string? MealImage { get; set; }
    public string? MealDetails { get; set; }
    public int Quantity { get; set; }
    public decimal MealPrice { get; set; }
    public decimal ItemsPrice { get; set; }
    public decimal AddOnsPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
