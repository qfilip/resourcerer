using Resourcerer.Utilities.Cryptography;

namespace Resourcerer.Utilities;

public static class PasswordGenerator
{
    public static string Generate()
    {
        var randomString = MiniId.Generate(10);
        return Hasher.GetSha1Hash(randomString);
    }
}
