namespace Utility.Consts;

public class ImageSettings
{
    public static string SectionName = "ImageSettings";
    public long MaxFileSize { get; set; }
    public string[] AllowedExtensions { get; set; } = [];
}
