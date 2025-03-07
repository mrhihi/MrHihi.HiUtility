using MrHihi.HiUtility.Cryptography;

namespace MrHihi.HiUtility.Test;

public class UnitTest2
{
    [Fact]
    public void Test_RSA()
    {
        IAsymmetric rsa = new RSAcrypt();
        var keyPair = rsa.GenerateKeyPair();
        var data = "Hello World";
        var signature = rsa.Sign(data, keyPair.privateKey);
        var result = rsa.Verify(data, signature, keyPair.publicKey);
        Assert.True(result);
    }
}