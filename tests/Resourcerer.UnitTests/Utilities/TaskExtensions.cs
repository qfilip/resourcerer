namespace Resourcerer.UnitTests.Utilities;

internal static class TaskExtensions
{
    public static void Await(this Task t) => t.GetAwaiter().GetResult();
    public static T Await<T>(this Task<T> t) => t.GetAwaiter().GetResult();
}
