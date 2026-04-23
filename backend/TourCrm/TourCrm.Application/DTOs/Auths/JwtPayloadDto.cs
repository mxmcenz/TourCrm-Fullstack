namespace TourCrm.Application.DTOs.Auths;

public sealed record JwtPayloadDto
{
    public int UserId { get; init; }
    public string Email { get; init; } = default!;
    public int CompanyId { get; init; } 
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}