namespace Otlob.Core.Contracts.Files;

public class UploadFileRequest
{
    public IFormFile File { get; init; } = default!;
}
