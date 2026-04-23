using System.ComponentModel.DataAnnotations.Schema;
using TourCrm.Core.Entities.Roles;

namespace TourCrm.Core.Entities;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string? PasswordHash { get; set; }
    public string LastName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
    public string PhoneNumber { get; set; } = string.Empty;
    public string ConfirmationCode { get; set; } = string.Empty;
    public DateTime? ConfirmationCodeGeneratedAt { get; set; }
    public bool IsEmailConfirmed { get; set; }
    public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
    
    [NotMapped]
    public string FullName =>
        string.Join(" ", new[] { LastName, FirstName, MiddleName }.Where(s => !string.IsNullOrWhiteSpace(s)));
}