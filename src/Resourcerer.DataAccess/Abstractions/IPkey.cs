namespace Resourcerer.DataAccess.Abstractions;

public interface IPkey<T> where T : struct
{
    public T Id { get; set; }
}
