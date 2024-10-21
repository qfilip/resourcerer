using Resourcerer.DataAccess.Records;

namespace Resourcerer.DataAccess.Abstractions;

public interface IAuditedEntity
{
    public AuditRecord AuditRecord { get; set; }
}
