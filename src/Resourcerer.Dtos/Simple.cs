namespace Resourcerer.Dtos;

public class Simple<T> : IDto where T : struct
{
    public Simple(T data)
    {
        Data = data;
    }

    public T Data { get; }
}
