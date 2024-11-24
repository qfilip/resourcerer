using Resourcerer.Identity.Models;

namespace Resourcerer.UnitTests.Utilities;

internal static class DataFaking
{
    public static AppIdentity Identity(bool admin, Guid companyId) => 
        new(Guid.NewGuid(), "oo", "a@a.com", admin, companyId);
}
