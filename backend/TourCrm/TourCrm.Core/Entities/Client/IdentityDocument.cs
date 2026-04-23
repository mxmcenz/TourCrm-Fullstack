using System.ComponentModel.DataAnnotations;
using TourCrm.Core.Common;

namespace TourCrm.Core.Entities.Client;

public class IdentityDocument : BaseEntity
{
    public int? CitizenshipCountryId { get; set; }
    public int? ResidenceCountryId { get; set; }
    public int? ResidenceCityId { get; set; }

    [MaxLength(200)] public string? BirthPlace { get; set; }
    [MaxLength(50)] public string SerialNumber { get; set; } = string.Empty;
    [MaxLength(200)] public string IssuedBy { get; set; } = string.Empty;
    public DateOnly? IssueDate { get; set; }

    [MaxLength(50)] public string DocumentNumber { get; set; } = string.Empty;
    [MaxLength(50)] public string? PersonalNumber { get; set; }

    [MaxLength(300)] public string? RegistrationAddress { get; set; }
    [MaxLength(300)] public string? ResidentialAddress { get; set; }

    [MaxLength(200)] public string? MotherFullName { get; set; }
    [MaxLength(200)] public string? FatherFullName { get; set; }
    [MaxLength(500)] public string? ContactInfo { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
}