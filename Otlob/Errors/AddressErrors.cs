namespace Otlob.ApiErrors;

public record AddressErrors
{
    public static readonly Error ExistedAddress = new("AddressErrors.ExisteddAddress", "The address is already exist");
    public static readonly Error InvalidAddress = new("AddressErrors.InvalidAddress", "There is invalid address");
    public static readonly Error NoAddressExists = new("AddressErrors.NoAddressExists", "No address exists for the user");
    public static readonly Error DeliveryAddressNotFound =
        new("AddressErrors.DeliveryAddressNotFound", "No delivery address found select address as delivered");
    public static readonly Error NoDeliveryAddress =
        new("AddressErrors.NoDeliveryAddress", "Please set a delivery address before placing an order.");
}
