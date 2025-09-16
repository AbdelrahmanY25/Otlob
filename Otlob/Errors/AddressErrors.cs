namespace Otlob.Errors;

public static class AddressErrors
{
    public static Error ExisteddAddress = new("AddressErrors.ExisteddAddress", "The address is already exist");
    public static Error InvalidAddress = new("AddressErrors.InvalidAddress", "There is invalid address");
}
