using Microsoft.Extensions.Configuration;

namespace Resourcerer.Utilities;

public static class ConfigurationReader
{
    public static T Load<T>(IConfigurationSection section, string path)
    {
        var value = section.GetValue<T>(path);
        if (value == null)
            throw new InvalidOperationException($"Secret {path} not found");

        return value;
    }

    public static T LoadValidated<T>(IConfigurationSection section, string path, Func<T, bool> validator)
    {
        var value = Load<T>(section, path);
        if (!validator(value))
            throw new System.InvalidOperationException($"Validation for {section}-{path} failed");

        return value;
    }

    public static IConfigurationSection LoadSection(IConfiguration configuration, string path)
    {
        var section = configuration.GetSection(path);

        if (section == null)
            throw new InvalidOperationException($"Section {path} not found");

        return section;
    }
}
