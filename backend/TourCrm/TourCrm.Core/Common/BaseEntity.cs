using System.ComponentModel.DataAnnotations;

namespace TourCrm.Core.Common;

public abstract class BaseEntity
{
    public int Id { get; set; }
    public int CompanyId { get; set; }
    public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAtUtc { get; set; }
    public bool IsDeleted { get; set; }
    [Timestamp] public byte[]? RowVersion { get; set; }
}
