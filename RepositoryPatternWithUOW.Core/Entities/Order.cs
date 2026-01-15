namespace Otlob.Core.Entities;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int RestaurantId { get; set; }
    public int? PromoCodeId { get; set; }

    public string CustomerPhoneNumber { get; set; } = string.Empty;
    public string DeliveryAddress { get; set; } = string.Empty;
    public Point DeliveryAddressLocation { get; set; } = default!;
    public decimal SubPrice { get; set; }
    public decimal DeliveryFee { get; set; }
    public decimal ServiceFeePrice { get; set; }
    public decimal DiscountAmount { get; set; } = 0;
    public decimal TotalPrice { get; }
    public string? Notes { get; set; }
    public PaymentMethod Method {  get; set; }
    public CustomerCancelReason? CustomerCancelReason { get; set; }
    public RestaurantCancelReason? RestaurantCancelReason { get; set; }

    public DateTime OrderDate { get; set; } = DateTime.Now;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    
    public ApplicationUser User { get; set; } = default!;
    public Restaurant Restaurant { get; set; } = default!;
    public PromoCode? PromoCode { get; set; }
    public ICollection<OrderDetails> OrderDetails { get; set; } = [];
    public OrderRating? Rating { get; set; }
}

public enum OrderStatus
{
    Pending,
    Preparing,
    Shipped,
    Delivered,
    Cancelled
}

public enum CustomerCancelReason
{
    ChangedMind,
    FoundBetterPrice,
    OrderTookTooLong,
    Other
}

public enum RestaurantCancelReason
{
    OutOfStock,
    UnableToFulfillOrder,
    Other
}

public enum PaymentMethod
{
    Cash,
    Credit
}
