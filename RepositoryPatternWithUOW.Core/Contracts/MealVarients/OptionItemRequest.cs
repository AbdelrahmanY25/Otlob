namespace Otlob.Core.Contracts.MealVarients;

public class OptionItemRequest
{
    public string? Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public int DisplayOrder { get; init; }
    public bool IsPobular { get; init; }
    public string? ExistingImage { get; init; }
    public UploadImageRequest? ImageRequest { get; init; }
}
