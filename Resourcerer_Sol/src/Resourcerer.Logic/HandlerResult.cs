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

    public static HandlerResult<T> NotFound(string message)
    {
        return new HandlerResult<T>(HandlerResultStatus.NotFound, new string[] { message });
    }

    public static HandlerResult<T> NotFound()
    {
        return new HandlerResult<T>(HandlerResultStatus.NotFound, Array.Empty<string>());
    }

    public static HandlerResult<T> ValidationError(string error)
    {
        return new HandlerResult<T>(HandlerResultStatus.ValidationError, new string[] { error });
    }

    public static HandlerResult<T> ValidationError(string[] errors)
    {
        return new HandlerResult<T>(HandlerResultStatus.ValidationError, errors);
    }

    private HandlerResult(T? obj)
    {
        Object = obj;
        Status = HandlerResultStatus.Ok;
        Errors = Array.Empty<string>();
    }

    private HandlerResult(HandlerResultStatus status, string[] errors)
    {
        Status = status;
        Errors = errors;
    }

    public T? Object { get; private set; }
    public string[] Errors { get; private set; }
    public HandlerResultStatus Status { get; private set; }

    public HandlerResult<TTarget> MapSelfTo<TTarget>()
    {
        return new HandlerResult<TTarget>(Status, Errors);
    }
}
