namespace Resourcerer.DataAccess.Records;

public class ReadOnlyAudit
{
    public Guid CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
}
