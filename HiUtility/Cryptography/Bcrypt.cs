namespace MrHihi.HiUtility.Cryptography;

public class Bcrypt: IHash
{
    public string Hash(string input)
    {
        return BCrypt.Net.BCrypt.HashPassword(input);
    }
    public bool Verify(string input, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(input, hash);
    }
}