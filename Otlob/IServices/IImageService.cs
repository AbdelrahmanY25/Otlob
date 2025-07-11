namespace Otlob.Core.IServices
{
    public interface IImageService
    {
        UploadImageResult UploadImage(IFormFile formFile);
        UploadImageResult DeleteOldImageIfExist(string? oldImage);
    }
}
