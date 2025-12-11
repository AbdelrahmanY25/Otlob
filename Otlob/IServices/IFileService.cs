namespace Otlob.IServices;

public interface IFileService
{
    Result<string> UploadImage(IFormFile formFile);
    Task<string> UploadFileAsync(IFormFile file);
    Task<(byte[] fileContent, string contentType, string fileName)> DownLoadFileAsync(string id);
    Result<string> DeleteImageIfExist(string? oldImage);
}
