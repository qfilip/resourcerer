namespace Resourcerer.Api;

public class AppInitializer
{
    public static string GetDbConnection(IWebHostEnvironment env)
    {
        var dbPath = Path.Combine(env.WebRootPath, "resourcerer.db3");
        return $"Datasource={dbPath}";
    }
}

