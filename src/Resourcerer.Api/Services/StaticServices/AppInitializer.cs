namespace Resourcerer.Api.Services.StaticServices;

public class AppInitializer
{
    public static string GetDbConnection(IWebHostEnvironment env)
    {
        var dbPath = Path.Combine(env.WebRootPath, "resourcerer.db3");
        return $"Datasource={dbPath}";
    }

    public static void LoadAuthConfiguration(IConfiguration configuration)
    {
        AppStaticData.Auth.Load(Load<bool>(configuration, "Auth", "Enabled"));

        var issuer = LoadRequired<string>(configuration, "Auth", "Issuer");
        var audience = LoadRequired<string>(configuration, "Auth", "Audience");
        var secretKey = LoadRequired<string>(configuration, "Auth", "JwtSecret");

        AppStaticData.Auth.Jwt.Configure(secretKey, issuer, audience);
    }

    public static T LoadRequired<T>(IConfiguration configuration, string section, string key)
    {
        var value = Load<T>(configuration, section, key, true);
        return value!;
    }

    public static T? Load<T>(IConfiguration configuration, string section, string key, bool required = false)
    {
        var value = configuration.GetSection(section).GetValue<T>(key);
        if (value == null && required)
        {
            throw new InvalidOperationException($"Secret {section}:{key} wasn't found");
        }

        return value;
    }
}

