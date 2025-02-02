namespace Otlob.Core.IServices
{
    public interface IIdEncryptionService
    {
        string EncryptId(int id);
        int DecryptId(string encryptedId);
    }
}
