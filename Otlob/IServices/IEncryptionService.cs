namespace Otlob.IServices;

public interface IEncryptionService
{
    string Encrypt(int id);
    int Decrypt(string encryptedId);
}
