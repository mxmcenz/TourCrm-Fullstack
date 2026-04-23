namespace TourCrm.Application.DTOs.Dictionaries.Hotels;

public sealed record HotelDto(int Id, string Name, int CityId, int? Stars);