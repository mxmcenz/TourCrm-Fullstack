using TourCrm.Application.Interfaces;

namespace TourCrm.Infrastructure.Services;

public class BCryptPasswordHasher : IPasswordHasher
{
    public string Hash(string password)
        => BCrypt.Net.BCrypt.HashPassword(password);

    public bool Verify(string hash, string password)
        => BCrypt.Net.BCrypt.Verify(password, hash);
}