namespace Otlob.IServices
{
    public interface IEncryptionService
    {
        string EncryptId(int id);
        int DecryptId(string encryptedId);
    }
}
