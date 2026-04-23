using System.ComponentModel.DataAnnotations;
using TourCrm.Core.Common;

namespace TourCrm.Core.Entities.Client;

public class Passport : BaseEntity
{
    [MaxLength(100)] public string FirstNameLatin { get; set; } = string.Empty;
    [MaxLength(100)] public string LastNameLatin { get; set; } = string.Empty;

    [MaxLength(50)] public string SerialNumber { get; set; } = string.Empty;

    public DateOnly? IssueDate { get; set; }
    public DateOnly? ExpireDate { get; set; }

    [MaxLength(200)] public string IssuingAuthority { get; set; } = string.Empty;

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}