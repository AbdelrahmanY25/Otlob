namespace Otlob.Core.Contracts.Address;

public class AddressResponse
{
    public string? Key { get; set; } = string.Empty;
    public string CustomerAddress { get; set; } = string.Empty;
    public PlaceType PlaceType { get; set; }
    public string StreetName { get; set; } = null!;
    public string? HouseNumberOrName { get; set; }
    public int? FloorNumber { get; set; }
    public string? CompanyName { get; set; }
    public double LonCode { get; init; }
    public double LatCode { get; init; }
    public bool IsDeliveryAddress { get; init; }
}
