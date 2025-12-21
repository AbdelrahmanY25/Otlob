namespace Otlob.Core.Contracts.MealVarients;

public class OptionGroupResponse
{
    public string Id { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public int DisplayOrder { get; init; }
    public IEnumerable<OptionItemResponse> OptionItems { get; set; } = default!;
}
