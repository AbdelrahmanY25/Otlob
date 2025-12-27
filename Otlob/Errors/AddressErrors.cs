namespace Otlob.Errors;

public record AddressErrors
{
    public static readonly Error ExistedAddress = new("AddressErrors.ExisteddAddress", "The address is already exist");
    public static readonly Error InvalidAddress = new("AddressErrors.InvalidAddress", "There is invalid address");
    public static readonly Error NoAddressExists = new("AddressErrors.NoAddressExists", "No address exists for the user");
}
