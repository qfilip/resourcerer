using Resourcerer.DataAccess.Entities;

namespace Resourcerer.DataAccess.AuthService;

public class AppDbIdentity
{
    public AppUser User { get; private set; } = new();
    public void SetUser(AppUser user) => User = user;
}
