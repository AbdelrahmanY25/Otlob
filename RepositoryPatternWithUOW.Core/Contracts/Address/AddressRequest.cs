namespace Otlob.Core.Contracts.Address;

public class AddressRequest
{
    public string CustomerAddress { get; set; } = string.Empty;
    public PlaceType PlaceType { get; set; }
    public string StreetName { get; set; } = null!;
    public string? HouseNumberOrName { get; set; }
    public int? FloorNumber { get; set; }
    public string? CompanyName { get; set; }
    public double LonCode { get; set; }
    public double LatCode { get; set; }
}
