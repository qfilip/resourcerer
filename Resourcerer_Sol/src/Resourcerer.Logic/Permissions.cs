using System.Text.Json;

namespace Resourcerer.Logic;

public class Permission
{
    public string? ForSet { get; set; }
    public int Level { get; set; }

    public static List<Permission> FromString(string value)
    {

    }

    public void SetLEvel(ePermission level)
    {
        Level = (int)level;
    }

    public string Serialize() => JsonSerializer.Serialize(this);
}

[Flags]
public enum ePermission
{
    Read = 1,
    Write = 2,
    Remove = 4
}
