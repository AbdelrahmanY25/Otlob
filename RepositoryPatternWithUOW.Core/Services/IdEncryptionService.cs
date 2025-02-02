using Microsoft.AspNetCore.WebUtilities;
using Otlob.Core.IServices;

namespace Otlob.Core.Services
{
    public class IdEncryptionService : IIdEncryptionService
    {
        public string EncryptId(int id)
        {
            var bytes = BitConverter.GetBytes(id);
            return WebEncoders.Base64UrlEncode(bytes);
        }
        public int DecryptId(string encryptedId)
        {
            var bytes = WebEncoders.Base64UrlDecode(encryptedId);
            return BitConverter.ToInt32(bytes, 0);
        }

    }
}
