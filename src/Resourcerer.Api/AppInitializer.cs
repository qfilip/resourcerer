namespace Resourcerer.Api;

public class AppInitializer
{
    public static string GetDbConnection(IWebHostEnvironment env)
    {
        var dbPath = Path.Combine(env.WebRootPath, "resourcerer.db3");
        return $"Datasource={dbPath}";
    }
    
    public static void LoadConfiguration(IConfiguration configuration)
    {
        AppStaticData.Auth.Jwt.SetJwtSecretKey(Load<string>(configuration, "Auth", "JwtSecret"));
        AppStaticData.Auth.Enabled = Load<bool>(configuration, "Auth", "Enabled");
    }

    private static T Load<T>(IConfiguration configuration, string section, string key)
    {
        var value = configuration.GetSection(section).GetValue<T>(key);
        if(value == null)
        {
            throw new InvalidOperationException($"Secret {section}:{key} wasn't found");
        }

        return value;
    }
}

