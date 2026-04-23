using System.ComponentModel.DataAnnotations;
using TourCrm.Core.Common;

namespace TourCrm.Core.Entities.Client;

public class BirthCertificate : BaseEntity
{
    [MaxLength(50)] public string SerialNumber { get; set; } = string.Empty;
    [MaxLength(200)] public string IssuedBy { get; set; } = string.Empty;
    public DateOnly? IssueDate { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}