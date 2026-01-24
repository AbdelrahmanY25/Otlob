namespace Otlob.Core.Contracts.MobileApp.Address;

public record UserAddressRequest
(
    double LonCode,
    double LatCode,
    string? GovermentOrCity,
    string? District,
    string StreetName,
    string? BuildingName,
    PropertyType PropertyType,
    string? Floor,
    string? FloorNo,
    string? AddressLabel,
    bool IsDefault,
    string? PhoneNumber,
    string? Instructions
);
