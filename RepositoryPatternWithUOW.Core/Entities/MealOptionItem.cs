namespace Otlob.Core.Entities;

public class MealOptionItem
{
    public string MealOptionItemId { get; set; } = Guid.CreateVersion7().ToString();
    public string OptionGroupId { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public string? Image { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsPobular { get; set; } = false;

    public MealOptionGroup OptionGroup { get; set; } = default!;
}
