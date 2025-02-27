using Microsoft.AspNetCore.WebUtilities;
using Otlob.Core.IServices;

namespace Otlob.Core.Services
{
    public class EncryptionService : IEncryptionService
    {
        private const int theSecretNumber = 897599;

        //private Random _theRandomSecretNumber;

        //public EncryptionService()
        //{
        //    _theRandomSecretNumber = new Random();
        //}

        public string EncryptId(int id)
        {
            long encryptedValue = (long)id * theSecretNumber;
            var bytes = BitConverter.GetBytes(encryptedValue);
            return WebEncoders.Base64UrlEncode(bytes);
        }
        public int DecryptId(string encryptedId)
        {
            var bytes = WebEncoders.Base64UrlDecode(encryptedId);
            var result = BitConverter.ToInt64(bytes, 0);
            return (int)(result / theSecretNumber);
        }

    }
}
