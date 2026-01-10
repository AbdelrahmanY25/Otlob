namespace Otlob.Core.Contracts.RestaurantOrders;

public class OrderUserInfoResponse
{
    public string UserId { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhoneNumber { get; init; } = string.Empty;
    public string? UserImage { get; init; }
}
