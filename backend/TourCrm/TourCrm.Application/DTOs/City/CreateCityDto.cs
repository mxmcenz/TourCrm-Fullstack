namespace TourCrm.Application.DTOs.City;

public class CreateCityDto
{
    public string Name { get; set; } = string.Empty;
    public int CountryId { get; set; }
}