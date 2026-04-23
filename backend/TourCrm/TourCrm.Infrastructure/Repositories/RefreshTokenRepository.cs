using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class RefreshTokenRepository(TourCrmDbContext context) : IRefreshTokenRepository
{
    public async Task AddAsync(RefreshToken token)
        => await context.RefreshTokens.AddAsync(token);

    public async Task<RefreshToken?> GetByTokenAsync(string token)
        => await context.RefreshTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.Token == token && !t.IsRevoked);
}