using System.Security.Cryptography;
using System.Text;

namespace Resourcerer.Utilities.Cryptography;
public class Hasher
{
    public static string GetSha1Hash(string plainText)
    {
        using var sha = SHA1.Create();
        return GetHash(plainText, sha);
    }

    public static string GetSha256Hash(string plainText)
    {
        using var sha = SHA256.Create();
        return GetHash(plainText, sha);
    }

    public static string GetSha512Hash(string plainText)
    {
        using var sha = SHA512.Create();
        return GetHash(plainText, sha);
    }

    private static string GetHash(string plainText, HashAlgorithm algorithm)
    {
        var hash = new StringBuilder();
        byte[] crypto = algorithm.ComputeHash(Encoding.UTF8.GetBytes(plainText));
        foreach (byte theByte in crypto)
        {
            hash.Append(theByte.ToString("x2"));
        }
        return hash.ToString();
    }
}

