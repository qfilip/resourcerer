using Resourcerer.DataAccess.Records;

namespace Resourcerer.Dtos.Records;

public class AuditDto : IDto
{
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public Guid CreatedBy { get; set; }
    public Guid ModifiedBy { get; set; }

    public static AuditDto Map(Audit audit) => new AuditDto
    {
        CreatedAt = audit.CreatedAt,
        ModifiedAt = audit.ModifiedAt,
        CreatedBy = audit.CreatedBy,
        ModifiedBy = audit.ModifiedBy
    };
}