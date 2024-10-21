namespace Resourcerer.DataAccess.Abstractions;

public interface IId<T>
{
    public T Id { get; set; }
}
