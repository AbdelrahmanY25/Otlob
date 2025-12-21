namespace Otlob.Core.Contracts.MealVarients;

public class OptionGroupRequest
{
    public string? Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public List<OptionItemRequest> OptionItems { get; init; } = [];
}
