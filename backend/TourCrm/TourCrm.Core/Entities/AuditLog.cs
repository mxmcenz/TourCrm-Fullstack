using TourCrm.Core.Enums;

namespace TourCrm.Core.Entities;

public class AuditLog
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public string Entity { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public AuditAction Action { get; set; }
    public string DataJson { get; set; } = "{}";
    public int? UserId { get; set; }
    public DateTime AtUtc { get; set; } = DateTime.UtcNow;
}
