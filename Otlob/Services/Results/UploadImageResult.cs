namespace Otlob.Services.Results
{
    public class UploadImageResult
    {
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
        public string ImageUrl { get; set; }

        public UploadImageResult(bool isSuccess, string? message = "", string imageUrl = "")
        {
            Message = message;
            IsSuccess = isSuccess;
            ImageUrl = imageUrl;
        }
    }
}
