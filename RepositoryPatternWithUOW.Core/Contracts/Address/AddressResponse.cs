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
}
