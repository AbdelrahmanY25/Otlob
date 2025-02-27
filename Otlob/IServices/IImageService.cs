using Microsoft.AspNetCore.Http;
using Otlob.Core.Models;


namespace Otlob.Core.IServices
{
    public interface IImageService
    {
        Task<string>? UploadImage(IFormFileCollection formFile, ImageProp imageProp);
    }
}
