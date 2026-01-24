namespace Otlob.Core.Contracts.MobileApp.Order;

public record OrderRequest
(
    string RestaurantKey,
    string AddressKey,
    PaymentMethod PaymentMethod,
    IEnumerable<OrderDetailRequest> Items,
    string? SpecialNotes = null,
    string? PromoCode = null
);