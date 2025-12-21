namespace Otlob.Services;

public class EncryptionService : IEncryptionService
{
    private const int theSecretNumber = 8_975_099;

    public string Encrypt(int id)
    {
        long encryptedValue = (long)id * theSecretNumber;
        var bytes = BitConverter.GetBytes(encryptedValue);
        return WebEncoders.Base64UrlEncode(bytes);
    }

    public int Decrypt(string encryptedId)
    {
        var bytes = WebEncoders.Base64UrlDecode(encryptedId);
        var result = BitConverter.ToInt64(bytes, 0);
        return (int)(result / theSecretNumber);
    }

}
