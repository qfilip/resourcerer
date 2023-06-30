using Microsoft.AspNetCore.Http;

namespace Resourcerer.UnitTests.Utilities;

public static class ArrayAssert
{
    public static void Every(this IResult[] iResults, Action<IResult> assert)
    {
        foreach(var i in iResults)
            assert(i);
    }
}
