namespace Resourcerer.Logic;

public enum HandlerResultStatus
{
    Ok, NotFound, ValidationError
}

public class HandlerResult<T>
{
    public static HandlerResult<T> Ok(T obj)
    {
        return new HandlerResult<T>(obj);
    }

    public static HandlerResult<T> NotFound()
    {
        return new HandlerResult<T>(HandlerResultStatus.NotFound, new string[0] { });
    }

    public static HandlerResult<T> ValidationError(string error)
    {
        return new HandlerResult<T>(HandlerResultStatus.ValidationError, new List<string> { error });
    }

    public static HandlerResult<T> ValidationError(IEnumerable<string> errors)
    {
        return new HandlerResult<T>(HandlerResultStatus.ValidationError, errors.ToList());
    }

    private HandlerResult(T? obj)
    {
        Object = obj;
        Status = HandlerResultStatus.Ok;
        Errors = new();
    }

    private HandlerResult(HandlerResultStatus status, IEnumerable<string> errors)
    {
        Status = status;
        Errors = errors.ToList();
    }

    public T? Object { get; private set; }
    public List<string> Errors { get; private set; }
    public HandlerResultStatus Status { get; private set; }

    public HandlerResult<TTarget> MapSelfTo<TTarget>()
    {
        return new HandlerResult<TTarget>(Status, Errors);
    }
}
