using Utility;


namespace Otlob.Core.IServices
{
    public interface IImageService
    {
        Task<string>? UploadImage(IFormFileCollection formFile, ImageUrl? imageUrl);
    }
}
