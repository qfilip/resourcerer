namespace Resourcerer.Logic.V1_0.Functions.Common;

public static partial class Validator
{
    public static HandlerResult<T> Check<T>((Func<bool>, string)[] checks) where T : new()
    {
        var errors = new List<string>();
        foreach (var check in checks)
        {
            if(!check.Item1())
            {
                errors.Add(check.Item2);
            }
        }

        if(errors.Count > 0)
        {
            return HandlerResult<T>.Rejected(errors.ToArray());
        }
        else
        {
            return HandlerResult<T>.Ok(new T());
        }
    }
}
