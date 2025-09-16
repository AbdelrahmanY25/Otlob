namespace Otlob.Services
{
    public  class ImageService(IOptions<ImageSettings> imageSettings) : IImageService
    {
        private readonly ImageSettings imageSettings = imageSettings.Value;

        public Result<string> UploadImage(IFormFile image)
        {
            var isImageUploaded = IsImageUploaded(image);

            if (isImageUploaded.IsFailure)
            {
                return isImageUploaded;
            }

            var isValidateImageSize = ValidateImageSize(image);

            if (isValidateImageSize.IsFailure)
            {
                return isValidateImageSize;
            }

            var isValidateImageExtention = ValidateImageExtension(image);

            if (isValidateImageExtention.IsFailure)
            {
                return isValidateImageExtention;
            }

            var uploadImageResult = CreateImage(image);

            return uploadImageResult;
        }

        public Result<string> DeleteImageIfExist(string? oldImage)
        {
            if (oldImage is null)
            {
                return Result.Success("Nice to upload your first image");
            }

            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", oldImage!);

            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
                return Result.Success("Old image deleted successfully.");
            }

            return Result.Failure<string>(ImageErrors.InvalidImagePath);
        }        

        private Result<string> IsImageUploaded(IFormFile image)
            => image is null ? Result.Failure<string>(ImageErrors.ImageDoesNotUploaded) : Result.Success(string.Empty);
              
        private Result<string> ValidateImageSize(IFormFile image)
        {            
            if (image.Length is > 0 && image.Length <= imageSettings.MaxFileSize)
            {
                return Result.Success(string.Empty);
            }
            
            return Result.Failure<string>(ImageErrors.InvalidImageSize); 
        }

        private Result<string> ValidateImageExtension(IFormFile image)
        {                        
            if (imageSettings.AllowedExtensions.Contains(Path.GetExtension(image.FileName).ToLower(), StringComparer.InvariantCultureIgnoreCase))
            {
                return Result.Success(string.Empty);
            }

            return Result.Failure<string>(ImageErrors.InvalidImageExtensions(string.Join(", ", imageSettings.AllowedExtensions)));
        }

        private static Result<string> CreateImage(IFormFile image)
        {
            string theImageExtention = Path.GetExtension(image.FileName);

            string newImageName = $"{Guid.NewGuid()}{theImageExtention}";

            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", newImageName);

            using var fileStream = new FileStream(imagePath, FileMode.Create);
            image.CopyTo(fileStream);

            return Result.Success(newImageName);
        }        
    }
}
