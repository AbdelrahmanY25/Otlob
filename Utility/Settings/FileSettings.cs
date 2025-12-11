namespace Utility.Consts;

public static class FileSettings
{
    public const int MaxFileSizeInMB = 2;
    public const int MaxFileSizeInBytes = MaxFileSizeInMB * 1024 * 1024;
    public const int MaxFileNameLength = 100;
    public static readonly string[] BlockedSignatures = ["2F-2F", "4D-5A", "D0-CF"];
    public static readonly string[] AllowedImageExtensions  = [".jpg", ".jpeg", ".png"];
    public static readonly string[] AllowedFileExtensions  = [".pdf", ".jpg", ".jpeg", ".png"];
}
