using Resourcerer.DataAccess.Enums;

namespace Resourcerer.DataAccess.Abstractions;

public interface ISoftDeletable
{
    public eEntityStatus EntityStatus { get; set; }
}
