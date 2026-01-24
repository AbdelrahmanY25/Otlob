namespace Otlob.Core.Entities;

public class Address
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    
    public string CustomerAddress { get; set; } = string.Empty;
    public Point Location { get; set; } = default!;
    public PropertyType PropertyType { get; set; }
    public string StreetName { get; set; } = string.Empty;
    public string? HouseNumberOrName { get; set; }
    public int? FloorNumber { get; set; }
    public string? CompanyName { get; set; }
    public bool IsDeliveryAddress { get; set; }

    public string? GovermentOrCity { get; set; }
    public string? District { get; set; }
    public string? Floor { get; set; }
    public string? FloorNo { get; set; }
    public string? AddressLabel { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Instructions { get; set; }
    public bool IsDefault { get; set; }

    public ApplicationUser User { get; set; } = default!;
}

public enum PropertyType
{
    Apartment = 1,
    House,
    Office
}
