using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public class Methods
    {
        public static string? UploadImg(IFormFile PhotoUrl)
        {
            if (PhotoUrl.Length > 0)
            {
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(PhotoUrl.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\images", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    PhotoUrl.CopyTo(stream);
                }
                return fileName;
            }
            return null;
        }
    }
}
