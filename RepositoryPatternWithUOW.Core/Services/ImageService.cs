using Microsoft.AspNetCore.Http;
using Otlob.Core.IServices;
using Otlob.Core.Models;

namespace Otlob.Core.Services
{
    public  class ImageService : IImageService
    {
        private const long maxFileSize = 4 * 1024 * 1024;
        private readonly string[] allowedExtentions = { ".png", ".jpg", ".jpeg" };
        public bool UploadUserProfilePicture(IFormFileCollection formFile)
        {
            if (formFile.Count > 0)
            {
                var file = formFile.FirstOrDefault();
                return file is not null;
            }
            return false;
        }
      
        public string CreateNewImageExtention(IFormFile image, string path)
        {
            var logoName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), path, logoName);

            using (var stream = File.Create(fullPath))
                image.CopyTo(stream);

            return logoName;
        }

        public bool DelteOldImage(string logo, string path)
        {
            if (logo is null) return false;
            var oldPath = Path.Combine(Directory.GetCurrentDirectory(), path, logo);
            if (File.Exists(oldPath))
            {
                File.Delete(oldPath);
                return true;
            }
            return false;
        }

        public string? ValidateImageSizeAndExtension(IFormFile image)
        {
            if (image is null) return "Please select an image.";
            if (!(image.Length != 0 && image.Length <= maxFileSize)) return "The file size exceeds the 4MB limit.";
            if (!(allowedExtentions.Contains(Path.GetExtension(image.FileName).ToLowerInvariant()))) return "Invalid file type. Only .jpg, .jpeg, and .png are allowed.";            
            return null;
        }
        public async Task<bool> CopyImageToMemoryStream(IFormFile image, ApplicationUser user)
        {
            try
            {
                using var memoryStream = new MemoryStream();
                await image.CopyToAsync(memoryStream);
                user.ProfilePicture = memoryStream.ToArray();

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
