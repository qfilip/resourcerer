using System.Security.Cryptography;
using System.Text;

namespace Resourcerer.Utilities.Cryptography;
public class RsaFunctions
{
    private static SHA512 _sha512 = SHA512.Create();
    private static RSA? _rsa = null;
    
    public static RSA GenerateRsaKey()
    {
        if (_rsa == null)
        {
            _rsa = RSA.Create();
        }

        return _rsa;
    }
    
    public static string Encrypt(string plainText, RSAParameters publicKey)
    {
        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(publicKey);
            var data = Encoding.UTF8.GetBytes(plainText);
            var cypher = rsa.Encrypt(data, false);

            return Convert.ToBase64String(cypher);
        }
    }

    public static string Decrypt(string cypherText, RSAParameters privateKey)
    {
        var dataBytes = Convert.FromBase64String(cypherText);
        using (var rsa = new RSACryptoServiceProvider())
        {
            rsa.ImportParameters(privateKey);
            var plainText = rsa.Decrypt(dataBytes, false);

            return Encoding.UTF8.GetString(plainText);
        }
    }

    public static string? Sign(string message, RSAParameters privateKey)
    {
        var signedBytes = new byte[0];
        using (var rsa = new RSACryptoServiceProvider())
        {
            byte[] originalData = Encoding.UTF8.GetBytes(message);
            try
            {
                rsa.ImportParameters(privateKey);
                signedBytes = rsa.SignData(originalData, _sha512);
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
            }
            finally
            {
                //// Set the keycontainer to be cleared when rsa is garbage collected.
                rsa.PersistKeyInCsp = false;
            }
        }

        return Convert.ToBase64String(signedBytes);
    }

    public static bool Verify(string original, string signed, RSAParameters publicKey)
    {
        bool success = false;
        
        using var rsa = new RSACryptoServiceProvider();
        
        byte[] bytesToVerify = Encoding.UTF8.GetBytes(original);
        byte[] signedBytes = Convert.FromBase64String(signed);
        try
        {
            rsa.ImportParameters(publicKey);
            var hash = SHA512.Create();
            byte[] hashedData = hash.ComputeHash(signedBytes);

            success = rsa.VerifyData(bytesToVerify, _sha512, signedBytes);
        }
        catch (CryptographicException e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            rsa.PersistKeyInCsp = false;
        }
        
        return success;
    }
}