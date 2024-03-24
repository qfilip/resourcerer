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
        AppStaticData.Auth.Load(Load<bool>(configuration, "Auth", "Enabled"));
        
        var secretKey = Load<string>(configuration, "Auth", "JwtSecret");
        var issuer = Load<string>(configuration, "Auth", "Issuer");
        var audience = Load<string>(configuration, "Auth", "Audience");

        AppStaticData.Auth.Jwt.Configure(secretKey, issuer, audience); 
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

