namespace TourCrm.Application.DTOs.Leads;

public sealed record LeadPageDto
{
    public IReadOnlyList<LeadListItemDto> Items { get; init; } = Array.Empty<LeadListItemDto>();
    public int Page { get; init; }
    public int PageSize { get; init; }
    public long Total { get; init; }
}