namespace Otlob.Core.Contracts.Authentication;

public class MealVm
{
    public int MealVmId { get; set; }
    public int RestaurantId { get; set; }
    public string? Key { get; set; }

    [Required, MinLength(3)]
    public string Name { get; set; } = string.Empty;

    [Required, MinLength(3)]
    public string Description { get; set; } = string.Empty;

    [Required, Range(1, 50000)]
    public decimal Price { get; set; }

    [Required, Range(0, 20)]
    public int NumberOfServings { get; set; }

    [Required]
    public bool IsAvailable { get; set; }
    public bool IsNewMeal { get; set; }
    public bool IsTrendingMeal { get; set; }
    public string? Image { get; set; } 
}
