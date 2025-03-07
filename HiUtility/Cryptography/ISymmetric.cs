namespace MrHihi.HiUtility.Cryptography;

public interface ISymmetric
{
    public string Encrypt(string data, string key);
    public string Decrypt(string data, string key);
}
