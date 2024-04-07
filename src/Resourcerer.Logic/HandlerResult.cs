using System.Diagnostics;

namespace Resourcerer.Logic;

public enum eHandlerResultStatus
{
    Ok, NotFound, Rejected
}

public class HandlerResult<T>
{
    public static HandlerResult<T> Ok(T obj)
    {
        return new HandlerResult<T>(obj);
    }

    public static HandlerResult<T> NotFound(string message)
    {
        return new HandlerResult<T>(eHandlerResultStatus.NotFound, new string[] { message });
    }

    public static HandlerResult<T> NotFound()
    {
        return new HandlerResult<T>(eHandlerResultStatus.NotFound, Array.Empty<string>());
    }

    public static HandlerResult<T> Rejected(string error)
    {
        return new HandlerResult<T>(eHandlerResultStatus.Rejected, new string[] { error });
    }

    public static HandlerResult<T> Rejected(IEnumerable<string> errors)
    {
        return new HandlerResult<T>(eHandlerResultStatus.Rejected, errors.ToArray());
    }

    public HandlerResult<U> MorphTo<U>(string? notFoundMessage = null)
    {
        return this.Status switch
        {
            eHandlerResultStatus.Ok => throw new InvalidOperationException("Cannot morph from Ok status"),
            eHandlerResultStatus.NotFound => notFoundMessage == null ? HandlerResult<U>.NotFound() : HandlerResult<U>.NotFound(notFoundMessage),
            eHandlerResultStatus.Rejected => HandlerResult<U>.Rejected(this.Errors),
            _ => throw new UnreachableException($"Cannot remap result handler of status ${this.Status}")
        };
    }

    private HandlerResult(T? obj)
    {
        Object = obj;
        Status = eHandlerResultStatus.Ok;
        Errors = Array.Empty<string>();
    }

    private HandlerResult(eHandlerResultStatus status, string[] errors)
    {
        Status = status;
        Errors = errors;
    }

    public T? Object { get; private set; }
    public string[] Errors { get; private set; }
    public eHandlerResultStatus Status { get; private set; }

    public HandlerResult<TTarget> MapSelfTo<TTarget>()
    {
        return new HandlerResult<TTarget>(Status, Errors);
    }
}
