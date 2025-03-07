using System.Security.Cryptography;
using System.Text;

namespace MrHihi.HiUtility.Cryptography;

public class RSAcrypt: IAsymmetric
{

    public string Sign(string data, string privateKey)
    {
        using RSA rsa = RSA.Create();
        rsa.ImportParameters(ConvertFromBase64String(privateKey, true));
        return Convert.ToBase64String(rsa.SignData(Encoding.UTF8.GetBytes(data), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1));
    }

    public bool Verify(string data, string signature, string publicKey)
    {
        using RSA rsa = RSA.Create();
        rsa.ImportParameters(ConvertFromBase64String(publicKey, false));
        return rsa.VerifyData(Encoding.UTF8.GetBytes(data), Convert.FromBase64String(signature), HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }

    public (string publicKey, string privateKey) GenerateKeyPair()
    {
        using RSA rsa = RSA.Create();
        (string publicKey, string privateKey) result;
        result.publicKey = ConvertToBase64String(rsa.ExportParameters(false));
        result.privateKey = ConvertToBase64String(rsa.ExportParameters(true));
        return result;
    }


    public static string ConvertToBase64String(RSAParameters key)
    {
        if (key.Modulus == null || key.Exponent == null)
        {
            throw new System.ArgumentException("Invalid RSA key parameters.");
        }
        StringBuilder sb = new StringBuilder();
        sb.AppendLine(Convert.ToBase64String(key.Modulus));
        sb.AppendLine(Convert.ToBase64String(key.Exponent));
        
        if (key.D != null && key.P !=null && key.Q != null && key.DP != null && key.DQ != null && key.InverseQ != null )
        {
            sb.AppendLine(Convert.ToBase64String(key.D));
            sb.AppendLine(Convert.ToBase64String(key.P));
            sb.AppendLine(Convert.ToBase64String(key.Q));
            sb.AppendLine(Convert.ToBase64String(key.DP));
            sb.AppendLine(Convert.ToBase64String(key.DQ));
            sb.AppendLine(Convert.ToBase64String(key.InverseQ));
        }

        return sb.ToString();
    }

    public static RSAParameters ConvertFromBase64String(string keyString, bool includePrivate)
    {
        string[] keyParts = keyString.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        RSAParameters key = new RSAParameters
        {
            Modulus = Convert.FromBase64String(keyParts[0]),
            Exponent = Convert.FromBase64String(keyParts[1])
        };

        if (includePrivate)
        {
            key.D = Convert.FromBase64String(keyParts[2]);
            key.P = Convert.FromBase64String(keyParts[3]);
            key.Q = Convert.FromBase64String(keyParts[4]);
            key.DP = Convert.FromBase64String(keyParts[5]);
            key.DQ = Convert.FromBase64String(keyParts[6]);
            key.InverseQ = Convert.FromBase64String(keyParts[7]);
        }

        return key;
    }
}
