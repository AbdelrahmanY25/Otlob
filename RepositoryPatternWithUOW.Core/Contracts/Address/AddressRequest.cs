namespace Otlob.Core.Contracts.Address;

public class AddressRequest
{
    public string CustomerAddress { get; init; } = string.Empty;
    public PlaceType PlaceType { get; init; }
    public string StreetName { get; init; } = string.Empty;
    public string? HouseNumberOrName { get; init; }
    public int? FloorNumber { get; init; }
    public string? CompanyName { get; init; }
    public double LonCode { get; init; }
    public double LatCode { get; init; }
}
