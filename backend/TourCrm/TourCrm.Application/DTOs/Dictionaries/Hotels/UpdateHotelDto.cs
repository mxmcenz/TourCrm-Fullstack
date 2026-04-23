namespace TourCrm.Application.DTOs.Dictionaries.Hotels;

public sealed record UpdateHotelDto(string Name, int CityId, int? Stars);