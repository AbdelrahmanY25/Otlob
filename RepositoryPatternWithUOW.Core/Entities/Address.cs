namespace Otlob.Core.Entities;

public class Address
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    
    public string CustomerAddress { get; set; } = string.Empty;
    public Point Location { get; set; } = default!;
    public PlaceType PlaceType { get; set; }
    public string StreetName { get; set; } = string.Empty;
    public string? HouseNumberOrName { get; set; }
    public int? FloorNumber { get; set; }
    public string? CompanyName { get; set; }

    public ApplicationUser User { get; set; } = default!;
}

public enum PlaceType
{
    Apartment = 1,
    House,
    Office
}
