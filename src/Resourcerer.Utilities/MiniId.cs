namespace Resourcerer.Utilities;

public static class MiniId
{
    private const string chars = "abcdefghijklmnopqrstuvwxyz0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";

    private static Random rand = new Random();

    public static string Generate(int length = 7)
    {
        var arr = Enumerable.Range(0, length)
            .Select(_ => chars[rand.Next(0, chars.Length)]);

        return String.Join(null, arr);
    }
}
