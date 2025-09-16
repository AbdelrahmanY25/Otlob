namespace Otlob.Core.Contracts.Restaurant;

public class BusinessDetailsVM
{
    public BusinessType BusinessType { get; set; }
    public List<Category> Categories { get; set; } = [];
    public int NumberOfBranches { get; set; }
    public Role Role { get; set; }
}
