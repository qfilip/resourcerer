using Microsoft.AspNetCore.Http;
using Resourcerer.Logic.Models;

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

    public static void DataIntegrity(
        TestDbContext ctx,
        Action<TestDbContext> tegridyCheck)
    {
        ctx.Clear();
        tegridyCheck(ctx);
    }
}
