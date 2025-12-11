namespace Otlob.Core.Contracts.Branch;

public class BranchResponse
{
    public string Key { get; init; } = string.Empty;
    public string RestaurantKey { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string Address { get; init; } = string.Empty;    
    public double DeliveryRadiusKm { get; init; }
    public string MangerName { get; init; } = string.Empty;
    public string MangerPhone { get; init; } = string.Empty;
    public double LonCode { get; init; }
    public double LatCode { get; init; }
}
