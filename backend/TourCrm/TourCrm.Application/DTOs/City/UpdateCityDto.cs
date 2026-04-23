using System.Security.AccessControl;

namespace TourCrm.Application.DTOs.City;

public class UpdateCityDto
{
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }
}