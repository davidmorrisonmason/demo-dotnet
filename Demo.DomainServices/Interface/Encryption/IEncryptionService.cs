namespace Demo.DomainServices.Interface.Encryption;

public interface IEncryptionService
{
    string OneWayHash(string plainText);
    string Fingerprint(string plainText);
    bool Verify(string plainText, string hashedText);
}
