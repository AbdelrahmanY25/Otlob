namespace Otlob.Core.Contracts.Restaurant;

public class RegistResturantRequest
{
    public string OwnerEmail { get; init; } = string.Empty;
    public string BrandName { get; init; } = string.Empty;
    public string BrandEmail { get; init; } = string.Empty;    
    public string MobileNumber { get; init; } = string.Empty;
} 
