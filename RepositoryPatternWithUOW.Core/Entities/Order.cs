namespace Otlob.Core.Entities;

public class Order
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int RestaurantId { get; set; }

    public string UserAddress { get; set; } = string.Empty;
    public decimal TotalMealsPrice { get; set; }
    public decimal TotalTaxPrice { get; set; }
    public decimal TotalOrderPrice { get; set; }
    public string? Notes { get; set; }
    public PaymentMethod Method {  get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public OrderStatus Status { get; set; } = OrderStatus.Pending;

    [ValidateNever]
    public ApplicationUser User { get; set; } = null!;

    [ValidateNever]
    public Restaurant Restaurant { get; set; } = null!;

    [ValidateNever]
    public ICollection<OrderDetails> MealsInOrder { get; set; } = null!;
}

public enum OrderStatus
{
    Pending,
    Preparing,
    Shipped,
    Delivered
}

public enum PaymentMethod
{
    Cash,
    Credit
}
