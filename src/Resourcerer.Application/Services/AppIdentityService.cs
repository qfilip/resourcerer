namespace Resourcerer.Application.Services;

public class AppIdentityService<T> where T : class, new()
{
    public T User { get; private set; } = new();
    public void SetUser(T user) => User = user;
}
