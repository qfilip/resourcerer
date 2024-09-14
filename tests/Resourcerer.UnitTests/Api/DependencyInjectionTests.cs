using Resourcerer.Api.Services.StaticServices;

namespace Resourcerer.UnitTests.Api;

public class DependencyInjectionTests
{
    [Fact]
    public void CanResolveServices()
    {
        var _ = Webapi.Build(Array.Empty<string>());
    }
}
