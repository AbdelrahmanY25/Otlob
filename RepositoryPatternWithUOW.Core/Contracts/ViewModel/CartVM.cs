namespace Otlob.Core.Contracts.Authentication;

public class CartVM
{
    public int CartVMId { get; set; }
    public  int RestaurantId { get; set; }
    public  decimal RestaurantDeliveryFee { get; set; }
    public decimal TotalMealsPrice { get; set; }
    public decimal TotalPrice { get; set; }
    public string? Image { get; set; }

    [ValidateNever]
    public IEnumerable<OrderedMealsVM> Meals { get; set; } = [];

    [ValidateNever]
    public IEnumerable<AddressResponse> Addresses { get; set; } = [];
}
