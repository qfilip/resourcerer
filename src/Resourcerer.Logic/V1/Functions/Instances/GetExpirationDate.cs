using Resourcerer.DataAccess.Entities;

namespace Resourcerer.Logic.V1_0.Functions;

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
