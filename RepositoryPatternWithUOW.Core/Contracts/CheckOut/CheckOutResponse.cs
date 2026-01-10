namespace Otlob.Core.Contracts.CheckOut;

public class CheckOutResponse
{
    public AddressResponse Address { get; init; } = default!;
    public decimal DeliveryTime { get; init; }
    public decimal SubTotal { get; init; }
    public decimal DeliveryFee { get; init; }
    public decimal ServiceFee { get; init; }
}
