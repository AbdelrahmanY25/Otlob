namespace Otlob.Core.Services
{
    public  class ImageService : IImageService
    {
        private const long maxFileSize = 4 * 1024 * 1024;
        private readonly string[] allowedExtentions = { ".png", ".jpg", ".jpeg" };

        private UploadImageResult IsImageUploaded(IFormFile image) 
            => image is null ? new UploadImageResult(false, "There is no Image Uploaded") : new UploadImageResult(true);
              
        private UploadImageResult ValidateImageSize(IFormFile image)
        {            
            if (image.Length is > 0 and <= maxFileSize)
            {
                return new UploadImageResult(true);
            }
            
            return new UploadImageResult(false, "The file size exceeds the 4MB limit."); 
        }

        private UploadImageResult ValidateImageExtension(IFormFile image)
        {                        
            if (allowedExtentions.Contains(Path.GetExtension(image.FileName).ToLower(), StringComparer.InvariantCultureIgnoreCase))
            {
                return new UploadImageResult(true);
            }

            return new UploadImageResult(false, "The file extension is not allowed. Allowed extensions are: " + string.Join(", ", allowedExtentions));
        }

        private UploadImageResult CreateImage(IFormFile image)
        {
            string theImageExtention = Path.GetExtension(image.FileName);

            string newImageName = $"{Guid.NewGuid()}{theImageExtention}";

            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", newImageName);

            using var fileStream = new FileStream(imagePath, FileMode.Create);
            image.CopyTo(fileStream);

            return new UploadImageResult(true, imageUrl: newImageName);
        }

        public UploadImageResult DeleteOldImageIfExist(string? oldImage)
        {
            if (oldImage.IsNullOrEmpty())
            {
                return new UploadImageResult(false, "The image is Null");
            }

            string imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", oldImage!);

            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
                return new UploadImageResult(true, "Old image deleted successfully.");
            }

            return new UploadImageResult(false, "The old image does not exist.");
        }


        public UploadImageResult UploadImage(IFormFile image)
        {        
            var isImageUploaded = IsImageUploaded(image);

            if (!isImageUploaded.IsSuccess)
            {
                return isImageUploaded;
            }

            var isValidateImageSize = ValidateImageSize(image);

            if (!isValidateImageSize.IsSuccess)
            {
                return isValidateImageSize;
            }

            var isValidateImageExtention = ValidateImageExtension(image);

            if (!isValidateImageExtention.IsSuccess)
            {
                return isValidateImageExtention;
            }
            
            var uploadImageResult = CreateImage(image);

            return uploadImageResult;
        }        
    }
}
