namespace Otlob.Core.Contracts.MobileApp.Order;

public record OrdersHistoreResponse
(
    int Id,
    string RestaurantName,
    DateTime OrderDate,
    decimal TotalAmount,
    OrderStatus Status
);