namespace Otlob.Core.Contracts.Files;

public class UploadImageRequest
{
    public IFormFile Image { get; init; } = default!;
}
