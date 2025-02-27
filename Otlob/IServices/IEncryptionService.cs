namespace Otlob.Core.IServices
{
    public interface IEncryptionService
    {
        string EncryptId(int id);
        int DecryptId(string encryptedId);
    }
}
