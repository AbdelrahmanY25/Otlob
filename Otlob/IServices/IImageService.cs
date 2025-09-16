namespace Otlob.IServices
{
    public interface IImageService
    {
        Result<string> UploadImage(IFormFile formFile);
        Result<string> DeleteImageIfExist(string? oldImage);
    }
}
