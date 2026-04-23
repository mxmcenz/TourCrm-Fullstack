using Microsoft.EntityFrameworkCore;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Deals;
using TourCrm.Core.Interfaces;
using TourCrm.Core.Specifications;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class DealRepository(TourCrmDbContext db) : GenericRepository<Deal>(db), IDealRepository
{
    public async Task<List<Deal>> GetAllWithIncludesAsync(
        int? statusId = null, int? officeId = null, int? managerId = null, CancellationToken ct = default)
    {
        var q = _context.Set<Deal>()
            .Include(x => x.Status)
            .Include(x => x.Manager)
            .Include(x => x.Company)
            .Include(x => x.IssuerLegalEntity)
            .Include(x => x.RequestType)
            .Include(x => x.Source)
            .Include(x => x.TourOperator)
            .Include(x => x.Customers)
            .Include(x => x.Tourists)
            .Include(x => x.Services)
            .Include(x => x.ClientPayments)
            .Include(x => x.PartnerPayments)
            .Include(d => d.RequestType)   
            .Include(d => d.Lead).ThenInclude(l => l!.Selections)
            .AsQueryable();

        if (statusId.HasValue) q = q.Where(x => x.StatusId == statusId.Value);
        if (managerId.HasValue) q = q.Where(x => x.ManagerId == managerId.Value);

        return await q.OrderByDescending(x => x.CreatedAt).ToListAsync(ct);
    }

    public Task<Deal?> GetByIdWithIncludesAsync(int id, CancellationToken ct = default) =>
        _context.Set<Deal>()
            .Include(x => x.Status)
            .Include(x => x.Manager)
            .Include(x => x.Company)
            .Include(x => x.IssuerLegalEntity)
            .Include(x => x.RequestType)
            .Include(x => x.Source)
            .Include(x => x.TourOperator)
            .Include(x => x.Customers)
            .Include(x => x.Tourists)
            .Include(x => x.Services)
            .Include(x => x.ClientPayments)
            .Include(x => x.PartnerPayments)
            .Include(d => d.RequestType)   
            .Include(d => d.Lead).ThenInclude(l => l!.Selections)
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task ChangeStatusAsync(int dealId, int statusId, CancellationToken ct = default)
    {
        var updated = await _context.Set<Deal>()
            .Where(x => x.Id == dealId)
            .ExecuteUpdateAsync(s => s
                .SetProperty(x => x.StatusId, statusId)
                .SetProperty(x => x.UpdatedAt, DateTimeOffset.UtcNow), cancellationToken: ct);
        if (updated == 0)
            throw new KeyNotFoundException("Deal not found");
    }

    public async Task<PagedResult<Deal>> SearchAsync(DealSearchRequest req, CancellationToken ct = default)
    {
        IQueryable<Deal> q = _context.Deals
            .AsNoTracking()
            .IgnoreQueryFilters()
            .Include(d => d.Status)
            .Include(d => d.Manager)
            .Include(d => d.Company)
            .Include(d => d.Tourists)
            .Include(d => d.RequestType)   
            .Include(d => d.Lead).ThenInclude(l => l!.Selections);

        var scope = (req.Scope ?? "all").ToLowerInvariant();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        switch (scope)
        {
            case "archived":
                q = q.Where(d => d.IsDeleted);
                break;

            case "active":
                q = q.Where(d => !d.IsDeleted && (!d.EndDate.HasValue || d.EndDate.Value >= today));
                break;

            default: // "all"
                q = q.Where(d => !d.IsDeleted);
                break;
        }

        if (!string.IsNullOrWhiteSpace(req.Query))
        {
            var term = req.Query.Trim();
            q = q.Where(d =>
                (d.DealNumber ?? "").Contains(term) ||
                (d.InternalNumber ?? "").Contains(term) ||
                d.Manager.FullName.Contains(term) ||
                d.Tourists.Any(t =>
                    t.FirstName.Contains(term) ||
                    t.LastName.Contains(term) ||
                    (t.FirstName + " " + t.LastName).Contains(term)));
        }

        if (!string.IsNullOrWhiteSpace(req.DealNumber)) q = q.Where(d => (d.DealNumber ?? "").Contains(req.DealNumber));
        if (!string.IsNullOrWhiteSpace(req.Booking)) q = q.Where(d => (d.BookingNumbers ?? "").Contains(req.Booking));
        if (req.StatusId is { } sid) q = q.Where(d => d.StatusId == sid);
        if (req.ManagerId is { } mid) q = q.Where(d => d.ManagerId == mid);

        if (req.CreatedFrom.HasValue)
        {
            var from = new DateTimeOffset(DateTime.SpecifyKind(req.CreatedFrom.Value, DateTimeKind.Utc));
            q = q.Where(d => d.CreatedAt >= from);
        }

        if (req.CreatedTo.HasValue)
        {
            var to = new DateTimeOffset(DateTime.SpecifyKind(req.CreatedTo.Value, DateTimeKind.Utc));
            q = q.Where(d => d.CreatedAt <= to);
        }
        
        if (req.StartFrom.HasValue)
        {
            var v = DateOnly.FromDateTime(req.StartFrom.Value.Date);
            q = q.Where(d => d.StartDate >= v);
        }

        if (req.StartTo.HasValue)
        {
            var v = DateOnly.FromDateTime(req.StartTo.Value.Date);
            q = q.Where(d => d.StartDate <= v);
        }

        if (req.EndFrom.HasValue)
        {
            var v = DateOnly.FromDateTime(req.EndFrom.Value.Date);
            q = q.Where(d => d.EndDate >= v);
        }

        if (req.EndTo.HasValue)
        {
            var v = DateOnly.FromDateTime(req.EndTo.Value.Date);
            q = q.Where(d => d.EndDate <= v);
        }

        if (req.ClientPaymentFrom.HasValue)
        {
            var v = DateOnly.FromDateTime(req.ClientPaymentFrom.Value.Date);
            q = q.Where(d => d.ClientPaymentDeadline >= v);
        }

        if (req.ClientPaymentTo.HasValue)
        {
            var v = DateOnly.FromDateTime(req.ClientPaymentTo.Value.Date);
            q = q.Where(d => d.ClientPaymentDeadline <= v);
        }

        if (req.PartnerPaymentFrom.HasValue)
        {
            var v = DateOnly.FromDateTime(req.PartnerPaymentFrom.Value.Date);
            q = q.Where(d => d.PartnerPaymentDeadline >= v);
        }

        if (req.PartnerPaymentTo.HasValue)
        {
            var v = DateOnly.FromDateTime(req.PartnerPaymentTo.Value.Date);
            q = q.Where(d => d.PartnerPaymentDeadline <= v);
        }

        if (!string.IsNullOrWhiteSpace(req.Tourist))
        {
            var t = req.Tourist.Trim();
            q = q.Where(d => d.Tourists.Any(cl =>
                cl.FirstName.Contains(t) || cl.LastName.Contains(t) ||
                (cl.FirstName + " " + cl.LastName).Contains(t)));
        }

        if (req.PriceFrom.HasValue) q = q.Where(d => d.Price >= req.PriceFrom.Value);
        if (req.PriceTo.HasValue) q = q.Where(d => d.Price <= req.PriceTo.Value);

        var desc = string.Equals(req.SortDir, "desc", StringComparison.OrdinalIgnoreCase);
        q = (req.SortBy ?? "").ToLowerInvariant() switch
        {
            "dealnumber" => desc ? q.OrderByDescending(d => d.DealNumber) : q.OrderBy(d => d.DealNumber),
            "internalnumber" => desc ? q.OrderByDescending(d => d.InternalNumber) : q.OrderBy(d => d.InternalNumber),
            "statusname" => desc ? q.OrderByDescending(d => d.Status!.Name) : q.OrderBy(d => d.Status!.Name),
            "startdate" => desc ? q.OrderByDescending(d => d.StartDate) : q.OrderBy(d => d.StartDate),
            "enddate" => desc ? q.OrderByDescending(d => d.EndDate) : q.OrderBy(d => d.EndDate),
            "managername" => desc ? q.OrderByDescending(d => d.Manager!.FullName) : q.OrderBy(d => d.Manager!.FullName),
            "price" => desc ? q.OrderByDescending(d => d.Price) : q.OrderBy(d => d.Price),
            _ => desc ? q.OrderByDescending(d => d.CreatedAt) : q.OrderBy(d => d.CreatedAt)
        };

        var total = await q.CountAsync(ct);
        var skip = Math.Max(0, (req.Page - 1) * req.PageSize);
        var items = await q.Skip(skip).Take(req.PageSize).ToListAsync(ct);
        return new PagedResult<Deal>(items, total);
    }

    public Task<Deal?> GetByIdIgnoringFiltersAsync(int id, CancellationToken ct = default) =>
        _context.Set<Deal>()
            .IgnoreQueryFilters()
            .Include(x => x.Status)
            .Include(x => x.Manager)
            .Include(x => x.Company)
            .Include(x => x.IssuerLegalEntity)
            .Include(x => x.RequestType)
            .Include(x => x.Source)
            .Include(x => x.TourOperator)
            .Include(d => d.Lead).ThenInclude(l => l!.Selections)
            .Include(x => x.Customers)
            .Include(x => x.Tourists)
            .Include(x => x.Services)
            .Include(x => x.ClientPayments)
            .Include(x => x.PartnerPayments)
            .FirstOrDefaultAsync(x => x.Id == id, ct);
}