using Microsoft.AspNetCore.Http;
using Resourcerer.Logic;
using System.Reflection;

namespace Resourcerer.UnitTests.Utilities;

public static class AssertUtils
{
    public static string[] Diffs<T>(T? expected, T? actual) where T : class
    {
        var nullDiff = (expected, actual) switch
        {
            (null, null) => false,
            (null, _) => true,
            (_, null) => true,
            (_, _) => false
        };

        if(nullDiff)
        {
            return ["One object is null"];
        }

        var diffs = new List<string>();
        var pubProps = typeof(T).GetProperties(BindingFlags.Public);
        
        for (int i = 0; i < pubProps.Length; i++)
        {
            var propertyType = pubProps[i].GetType();
            var expectedValue = pubProps[i].GetValue(expected);
            var actualValue = pubProps[i].GetValue(actual);

            if (propertyType.IsValueType)
            {
                if(expectedValue != actualValue)
                {
                    diffs.Add($"{propertyType.Name} values don't match");
                }
            }
            else
            {
                diffs.AddRange(Diffs(expectedValue, actualValue));
            }
        }

        return diffs.ToArray();
    }

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
