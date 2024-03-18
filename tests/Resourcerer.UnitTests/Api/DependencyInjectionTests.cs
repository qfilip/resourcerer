using Resourcerer.Api;

namespace Resourcerer.UnitTests.Api;

public class DependencyInjectionTests
{
    [Fact]
    public void CanResolveServices()
    {
        var _ = Webapi.Create(Array.Empty<string>());
    }
}
