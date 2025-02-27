using Otlob.Core.IServices;
using Otlob.Core.Models;

namespace Otlob.Core.Services
{
    public  class ImageService : IImageService
    {
        private const long maxFileSize = 4 * 1024 * 1024;
        private readonly string[] allowedExtentions = { ".png", ".jpg", ".jpeg" };

        private bool IsImageUploaded(IFormFileCollection formFile) => formFile.Count != 0;
              
        private string? ValidateImageSizeAndExtension(IFormFile image)
        {
            if (image is null)
            {
                return "Please select an image.";
            }

            if (!(image.Length != 0 && image.Length <= maxFileSize))
            {
                return "The file size exceeds the 4MB limit.";

            }

            if (!(allowedExtentions.Contains(Path.GetExtension(image.FileName).ToLowerInvariant())))
            {
                return "Invalid file type. Only .jpg, .jpeg, and .png are allowed.";
            }

            return null; 
        }

        private async Task<bool> CopyImageToMemoryStream(IFormFile image, ImageProp imageProp)
        {            
            using var memoryStream = new MemoryStream();
            await image.CopyToAsync(memoryStream);

            if (imageProp is null)
            {
                return false;
            }

            imageProp.Image = memoryStream.ToArray();           
            return true;
        }

        public async Task<string>? UploadImage(IFormFileCollection formFile, ImageProp? imageProp)
        {
            if(!IsImageUploaded(formFile))
            {
                return "There is no Image Uploaded";
            }

            var image = formFile.FirstOrDefault();
            var resOfValidation = ValidateImageSizeAndExtension(image);

            if (resOfValidation is string)
            {
                return resOfValidation;
            }
           
            if (imageProp is null)
            {
                return "Error in uploading image";
            }

            if (!await CopyImageToMemoryStream(image, imageProp: imageProp))
            {
                return "Error in uploading image";
            }

            return null;
        }        
    }
}
