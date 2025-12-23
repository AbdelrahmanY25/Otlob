namespace Otlob.Core.Contracts.MealVarients;

public class AddOnRequest
{
    public string Name { get; init; } = string.Empty;
    public decimal Price { get; init; }
    public UploadImageRequest ImageRequest { get; init; } = default!;
}
