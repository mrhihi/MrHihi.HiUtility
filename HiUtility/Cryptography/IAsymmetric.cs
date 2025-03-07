namespace MrHihi.HiUtility.Cryptography;

public interface IAsymmetric
{
    public string Sign(string data, string privateKey);
    public bool Verify(string data, string signature, string publicKey);
    public (string publicKey, string privateKey) GenerateKeyPair();
}
