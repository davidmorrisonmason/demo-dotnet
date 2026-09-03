namespace Demo.DomainServices.Interface.Encryption;

public interface IEncryptionService
{
    string OneWayHash(string plainText);
    bool Verify(string plainText, string hashedText);
}
