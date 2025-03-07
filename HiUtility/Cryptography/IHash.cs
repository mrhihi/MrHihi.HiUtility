namespace MrHihi.HiUtility.Cryptography;

public interface IHash
{
    public string Hash(string input);
    public bool Verify(string text, string hash);
}
