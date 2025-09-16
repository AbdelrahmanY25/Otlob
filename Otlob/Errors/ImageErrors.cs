namespace Otlob.Errors;

public static class ImageErrors
{
    public static readonly Error ImageDoesNotUploaded = new("ImageErrors.ImageDoesNotUploaded", "There is no Image Uploaded");
    
    public static readonly Error InvalidImageSize = new("ImageErrors.InvalidImageSize", "The file size exceeds the 4MB limit.");
    
    public static readonly Error InvalidImagePath = new("ImageErrors.InvalidImagePath", "The old image does not exist.");
    
    public static Error InvalidImageExtensions(string extensions) =>
        new("ImageErrors.InvalidImageExtensions", $"The file extension is not allowed. Allowed extensions are: {extensions}");
}
