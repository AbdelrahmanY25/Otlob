namespace Otlob.Core.Entities;

public class UploadedFile
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public string FileName { get; set; } = string.Empty;
    public string StoredFileName { get; set; } = string.Empty;
    public string ContetntType { get; set; } = string.Empty;
    public string FileExtentsion { get; set; } = string.Empty;
}
