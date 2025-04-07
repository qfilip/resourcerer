namespace Resourcerer.Identity.Enums;

[Flags]
public enum eAction
{
    View = 1,
    Create = 2,
    Update = 4,
    Remove = 8,
    Delete = 16
}
