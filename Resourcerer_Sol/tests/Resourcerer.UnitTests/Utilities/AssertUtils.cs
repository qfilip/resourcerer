using Microsoft.AspNetCore.Http;
using Resourcerer.Logic;

namespace Resourcerer.UnitTests.Utilities;

public static class AssertUtils
{
    public static void Every(this IResult[] iResults, Action<IResult> assert)
    {
        foreach(var i in iResults)
            assert(i);
    }

    public static void Every<T>(this HandlerResult<T>[] hResults, Action<HandlerResult<T>> assert)
    {
        foreach (var h in hResults)
            assert(h);
    }
}
