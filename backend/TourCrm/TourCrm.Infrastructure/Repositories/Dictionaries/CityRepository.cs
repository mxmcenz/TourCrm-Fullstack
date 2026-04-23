using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Interfaces.Dictionaries;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories.Dictionaries;

public class CityRepository : GenericRepository<City>, ICityRepository
{
    private readonly TourCrmDbContext _context;

    public CityRepository(TourCrmDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<City?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _context.Cities
            .Include(c => c.Country)
            .FirstOrDefaultAsync(c => c.Id == id, ct);
    }
    public async Task<IEnumerable<City>> GetAllAsync(CancellationToken ct = default)
    {
        return await _context.Cities
            .Include(c => c.Country) 
            .ToListAsync(ct);
    }
}