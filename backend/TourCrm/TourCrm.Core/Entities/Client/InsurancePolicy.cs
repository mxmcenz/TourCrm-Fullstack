using System.ComponentModel.DataAnnotations;
using TourCrm.Core.Common;

namespace TourCrm.Core.Entities.Client;

public class InsurancePolicy : BaseEntity
{
    public DateOnly? IssueDate { get; set; }
    public DateOnly? ExpireDate { get; set; }

    public int? CountryId { get; set; }

    [MaxLength(2000)] public string? Note { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}