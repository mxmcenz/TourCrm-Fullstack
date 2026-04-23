using System.Text.Json.Serialization;

namespace TourCrm.Application.DTOs.Deals.FiledsForCreate;

public sealed record NewTouristDto
{
    [JsonPropertyName("clientId")]
    public int? ClientId { get; init; }
    public string? FullName { get; init; }
    public string? Phone    { get; init; }
    public string? Email    { get; init; }
}