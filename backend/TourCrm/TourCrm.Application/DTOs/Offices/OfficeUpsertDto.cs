using System.ComponentModel.DataAnnotations;

namespace TourCrm.Application.DTOs.Offices;

public class OfficeUpsertDto
{
    [Required] public int LegalEntityId { get; set; }

    [Required, MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)] public string? Address { get; set; }
    [MaxLength(64)]  public string? Phone   { get; set; }
    [MaxLength(150)] public string? Email   { get; set; } 

    public int? LeadLimit { get; set; }
}