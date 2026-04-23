using System.Text.Json;

namespace TourCrm.Application.DTOs.Auths;

public sealed record AuditLogDto
{
    public int Id { get; init; }
    public int CompanyId { get; init; }
    public string Entity { get; init; } = string.Empty;
    public string EntityId { get; init; } = string.Empty;
    public string Action { get; init; } = string.Empty;
    public JsonElement Data { get; init; }
    public int? UserId { get; init; }
    public DateTime AtUtc { get; init; }
}