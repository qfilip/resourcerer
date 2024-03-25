namespace Resourcerer.Logic.V1.Functions;

public static partial class Instances
{
    public static DateTime? GetExpirationDate(double? itemExpirationSeconds, DateTime currentTime)
    {
        if(itemExpirationSeconds == null)
        {
            return null;
        }

        return currentTime.AddSeconds(itemExpirationSeconds.Value);
    }
}
