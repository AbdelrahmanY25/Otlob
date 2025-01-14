using Microsoft.AspNetCore.Http;
using Otlob.Core.Models;


namespace Otlob.Core.IServices
{
    public interface IImageService
    {
        string? ValidateImageSizeAndExtension(IFormFile image);
        string CreateNewImageExtention(IFormFile image, string path);
        bool DelteOldImage(string logo, string path);
        bool UploadUserProfilePicture(IFormFileCollection formFile);
        Task<bool> CopyImageToMemoryStream(IFormFile image, ApplicationUser user);
    }
}
