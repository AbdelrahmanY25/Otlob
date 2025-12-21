namespace Utility.Consts;

public static class FileSettings
{
    public const int MaxFileSizeInMB = 2;
    public const int MaxFileSizeInBytes = MaxFileSizeInMB * 1024 * 1024;
    public const int MaxFileNameLength = 100;
    
    // Allowed image signatures (jpg, png, jpeg, svg, webp)
    public static readonly string[] AllowedImageSignatures = 
    [
        "FF-D8-FF",        // JPEG/JPG
        "89-50-4E-47",     // PNG
        "3C-73-76-67",     // SVG (starts with '<svg')
        "3C-3F-78-6D",     // SVG (starts with '<?xm' for XML declaration)
        "52-49-46-46" // WEBP (RIFF....WEBP)
    ];
    
    // Allowed file signatures (jpg, png, jpeg, pdf)
    public static readonly string[] AllowedFileSignatures = 
    [
        "FF-D8-FF",        // JPEG/JPG
        "89-50-4E-47",     // PNG
        "25-50-44-46"      // PDF (starts with '%PDF')
    ];
    
    public static readonly string[] AllowedImageExtensions  = [".jpg", ".jpeg", ".png", ".svg", ".webp"];
    public static readonly string[] AllowedFileExtensions  = [".pdf", ".jpg", ".jpeg", ".png"];
}
