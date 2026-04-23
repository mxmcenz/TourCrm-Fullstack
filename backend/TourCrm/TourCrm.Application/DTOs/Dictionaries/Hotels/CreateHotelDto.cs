namespace TourCrm.Application.DTOs.Dictionaries.Hotels;

public sealed record CreateHotelDto(string Name, int CityId, int? Stars);