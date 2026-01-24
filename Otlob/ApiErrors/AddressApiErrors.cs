namespace Otlob.ApiErrors;

public static class AddressApiErrors
{
    public static readonly ApiError ExistedAddress = 
        new("AddressErrors.ExisteddAddress", "The address is already exist", StatusCodes.Status409Conflict);

    public static readonly ApiError InvalidAddress = new("AddressErrors.InvalidAddress", "There is invalid address", StatusCodes.Status400BadRequest);

    public static readonly ApiError NoAddressExists = new("AddressErrors.NoAddressExists", "No address exists for the user", StatusCodes.Status404NotFound);

    public static readonly ApiError DeliveryAddressNotFound =
        new("AddressErrors.DeliveryAddressNotFound", "No delivery address found select address as delivered", StatusCodes.Status404NotFound);

    public static readonly ApiError NoDeliveryAddress =
        new("AddressErrors.NoDeliveryAddress", "Please set a delivery address before placing an order.", StatusCodes.Status400BadRequest);

    public static readonly ApiError InvalidLatitudeOrLongitudeValues =
       new(Code: "Common.InvalidLatitudeOrLongitudeValues", Description: "Invalid latitude or longitude values.", StatusCodes.Status400BadRequest);
}
