using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities.Tariffs;
using TourCrm.Core.Interfaces;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class TariffRepository(TourCrmDbContext context) : GenericRepository<Tariff>(context), ITariffRepository
{
    private readonly TourCrmDbContext _ctx = context;

    public async Task<Tariff?> GetWithPermissionsAsync(int id, CancellationToken ct = default)
        => await _ctx.Tariffs
            .Include(t => t.Permissions)
            .FirstOrDefaultAsync(t => t.Id == id, ct);

    public async Task<Tariff?> GetByNameAsync(string name, CancellationToken ct = default)
        => await _ctx.Tariffs.FirstOrDefaultAsync(t => t.Name == name, ct);

    public async Task<bool> NameExistsAsync(string name, int? excludeId = null, CancellationToken ct = default)
        => await _ctx.Tariffs.AnyAsync(t => t.Name == name && (excludeId == null || t.Id != excludeId), ct);

    public async Task<Tariff?> FindByEmployeeCountAsync(int employees, CancellationToken ct = default)
        => await _ctx.Tariffs
            .OrderBy(t => t.MinEmployees)
            .FirstOrDefaultAsync(t => employees >= t.MinEmployees && employees <= t.MaxEmployees, ct);

    public async Task<IReadOnlyList<Tariff>> GetAllLightAsync(CancellationToken ct = default)
        => await _ctx.Tariffs
            .AsNoTracking()
            .OrderBy(t => t.MinEmployees)
            .ToListAsync(ct);
}