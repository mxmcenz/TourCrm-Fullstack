using System.Linq; // важно!
using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Leads;
using TourCrm.Core.Interfaces;
using TourCrm.Core.Specifications;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public sealed class LeadRepository : GenericRepository<Lead>, ILeadRepository
{
    private readonly TourCrmDbContext _ctx;
    private readonly DbSet<Lead> _db;

    public LeadRepository(TourCrmDbContext context) : base(context)
    {
        _ctx = context;
        _db = context.Set<Lead>();
    }

    public async Task<Lead?> GetAsync(int id, string userId, CancellationToken ct = default)
        => await _db
            .Include(l => l.LeadStatus)
            .Include(l => l.Source)
            .Include(l => l.RequestType)
            .Include(l => l.LeadLabels).ThenInclude(ll => ll.Label)
            .FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, ct);

    public async Task<Lead?> GetByNumberAsync(string leadNumber, string userId, CancellationToken ct = default)
        => await _db
            .Include(l => l.LeadStatus)
            .Include(l => l.Source)
            .Include(l => l.RequestType)
            .FirstOrDefaultAsync(l => l.LeadNumber == leadNumber && !l.IsDeleted, ct);

    public async Task<(IReadOnlyList<Lead> Items, long Total)> SearchAsync(
        LeadSearchSpec s, string userId, CancellationToken ct = default)
    {
        IQueryable<Lead> q = _db.AsNoTracking()
            .Include(l => l.LeadStatus)
            .Where(l => !l.IsDeleted);

        if (s.StatusId is { } st) q = q.Where(l => l.LeadStatusId == st);
        if (s.ManagerId is { } mid) q = q.Where(l => l.ManagerId == mid);
        if (s.OfficeId is { } oid) q = q.Where(l => l.OfficeId == oid);

        if (s.CreatedFrom is { } cf) q = q.Where(l => l.CreatedAt >= cf);
        if (s.CreatedTo is { } cto) q = q.Where(l => l.CreatedAt < cto);

        if (s.TravelFrom is { } tf) q = q.Where(l => l.DesiredArrival >= tf);
        if (s.TravelTo is { } tt) q = q.Where(l => l.DesiredArrival <= tt);

        if (s.NightsMin is { } nmin) q = q.Where(l => l.NightsFrom >= nmin);
        if (s.NightsMax is { } nmax) q = q.Where(l => l.NightsTo <= nmax);

        if (!string.IsNullOrWhiteSpace(s.Country))
        {
            var term = s.Country.Trim();
            if (_ctx.Database.IsNpgsql())
            {
                q = q.Where(l => l.Country != null &&
                                 EF.Functions.ILike(l.Country!, $"%{term}%"));
            }
            else
            {
                var termLower = term.ToLower();
                q = q.Where(l => l.Country != null &&
                                 l.Country!.ToLower().Contains(termLower));
            }
        }

        if (!string.IsNullOrWhiteSpace(s.Query))
        {
            var term = s.Query.Trim();
            if (_ctx.Database.IsNpgsql())
            {
                q = q.Where(l =>
                    EF.Functions.ILike(l.LeadNumber, $"%{term}%") ||
                    EF.Functions.ILike(l.CustomerFirstName, $"%{term}%") ||
                    EF.Functions.ILike(l.CustomerLastName, $"%{term}%") ||
                    (l.Phone != null && EF.Functions.ILike(l.Phone, $"%{term}%")) ||
                    (l.Email != null && EF.Functions.ILike(l.Email, $"%{term}%"))
                );
            }
            else
            {
                var termLower = term.ToLower();
                q = q.Where(l =>
                    l.LeadNumber.ToLower().Contains(termLower) ||
                    l.CustomerFirstName.ToLower().Contains(termLower) ||
                    l.CustomerLastName.ToLower().Contains(termLower) ||
                    (l.Phone != null && l.Phone!.ToLower().Contains(termLower)) ||
                    (l.Email != null && l.Email!.ToLower().Contains(termLower))
                );
            }
        }

        var desc = string.Equals(s.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
        q = (s.SortBy?.ToLower()) switch
        {
            "traveldate" => desc
                ? q.OrderByDescending(x => x.DesiredArrival).ThenByDescending(x => x.Id)
                : q.OrderBy(x => x.DesiredArrival).ThenBy(x => x.Id),

            "nights" => desc
                ? q.OrderByDescending(x => x.NightsFrom).ThenByDescending(x => x.Id)
                : q.OrderBy(x => x.NightsFrom).ThenBy(x => x.Id),

            "country" => desc
                ? q.OrderByDescending(x => x.Country).ThenByDescending(x => x.Id)
                : q.OrderBy(x => x.Country).ThenBy(x => x.Id),

            "budget" => desc
                ? q.OrderByDescending(x => x.Budget).ThenByDescending(x => x.Id)
                : q.OrderBy(x => x.Budget).ThenBy(x => x.Id),

            "status" => desc
                ? q.OrderByDescending(x => x.LeadStatus!.Name).ThenByDescending(x => x.Id)
                : q.OrderBy(x => x.LeadStatus!.Name).ThenBy(x => x.Id),

            "manager" => desc
                ? q.OrderByDescending(x => x.ManagerFullName).ThenByDescending(x => x.Id)
                : q.OrderBy(x => x.ManagerFullName).ThenBy(x => x.Id),

            _ => desc
                ? q.OrderByDescending(x => x.CreatedAt).ThenByDescending(x => x.Id)
                : q.OrderBy(x => x.CreatedAt).ThenBy(x => x.Id),
        };

        var total = await q.LongCountAsync(ct);
        var items = await q.Skip(Math.Max(0, (s.Page - 1) * s.PageSize))
            .Take(s.PageSize)
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task SoftDeleteAsync(int id, string userId, CancellationToken ct = default)
    {
        var lead = await _db.FirstOrDefaultAsync(l => l.Id == id && !l.IsDeleted, ct);
        if (lead is null) return;

        lead.IsDeleted = true;
        lead.UpdatedAt = DateTime.UtcNow;
    }
    
    public async Task<Dictionary<int, int>> GetLeadCountsByManagerAsync(int companyId, CancellationToken ct = default)
    {
        return await _ctx.Leads
            .AsNoTracking()
            .Where(l => l.CompanyId == companyId
                        && !l.IsDeleted
                        && l.ManagerId != null)
            .GroupBy(l => l.ManagerId!.Value)
            .Select(g => new { ManagerId = g.Key, Cnt = g.Count() })
            .ToDictionaryAsync(x => x.ManagerId, x => x.Cnt, ct);
    }
    
    public async Task<int> CountActiveByOfficeAsync(int companyId, int officeId, CancellationToken ct = default)
    {
        return await _ctx.Leads
            .AsNoTracking()
            .Where(l => l.CompanyId == companyId && l.OfficeId == officeId && !l.IsDeleted)
            .CountAsync(ct);
    }

    public async Task<Dictionary<int,int>> GetActiveLeadCountsByOfficeAsync(int companyId, CancellationToken ct = default)
    {
        return await _ctx.Leads
            .AsNoTracking()
            .Where(l => l.CompanyId == companyId && !l.IsDeleted)
            .GroupBy(l => l.OfficeId)
            .Select(g => new { OfficeId = g.Key, Cnt = g.Count() })
            .ToDictionaryAsync(x => x.OfficeId, x => x.Cnt, ct);
    }
}