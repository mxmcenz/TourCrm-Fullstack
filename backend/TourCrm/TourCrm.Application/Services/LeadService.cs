using System.Text.Json;
using TourCrm.Application.DTOs.Leads;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Abstractions;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Dictionaries;
using TourCrm.Core.Entities.Leads;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services;

public class LeadService(IUnitOfWork uow, ICompanyContext companyContext) : ILeadService
{
    private readonly int _companyId = companyContext.CompanyId;

    public async Task<IEnumerable<LeadDto>> GetAllAsync(CancellationToken ct = default)
    {
        var leads = await uow.Leads.GetAllAsync(ct);
        return leads.Select(ToDto);
    }

    public async Task<LeadDto?> GetByIdAsync(int id, CancellationToken ct = default)
        => (await uow.Leads.GetAsync(id, userId: "system", ct)) is { } l ? ToDto(l) : null;

    private static string BuildFullName(Employee e) =>
        string.Join(' ', new[] { e.LastName, e.FirstName, e.MiddleName }
            .Where(s => !string.IsNullOrWhiteSpace(s))).Trim();

    public async Task<LeadDto> CreateAsync(CreateLeadDto dto, CancellationToken ct = default)
    {
        if (dto.LeadStatusId <= 0) dto.LeadStatusId = 1;

        Employee manager =
            dto.ManagerId is > 0 and var mid
                ? (await uow.Users.GetByIdAsync(mid, ct) as Employee)
                  ?? throw new InvalidOperationException("Manager must be an employee with office")
                : await PickLeastBusyManagerAsync(ct);

        await EnsureOfficeCapacityOrThrowAsync(manager.OfficeId, ct);

        var mf = string.IsNullOrWhiteSpace(dto.ManagerFullName)
            ? (!string.IsNullOrWhiteSpace(manager.FullName) ? manager.FullName : BuildFullName(manager))
            : dto.ManagerFullName.Trim();

        var lead = new Lead
        {
            LeadNumber = GenerateLeadNumber(),
            LeadStatusId = dto.LeadStatusId,
            RequestTypeId = dto.RequestTypeId,
            SourceId = dto.SourceId,
            ManagerId = manager.Id,
            ManagerFullName = string.IsNullOrWhiteSpace(mf) ? null : mf,
            OfficeId = manager.OfficeId,
            CustomerType = dto.CustomerType,
            CustomerFirstName = dto.CustomerFirstName,
            CustomerLastName = dto.CustomerLastName,
            CustomerMiddleName = dto.CustomerMiddleName,
            Phone = dto.Phone,
            Email = dto.Email,
            Country = dto.Country,
            Adults = dto.Adults,
            Children = dto.Children,
            Infants = dto.Infants,
            DesiredArrival = dto.DesiredArrival,
            DesiredDeparture = dto.DesiredDeparture,
            NightsFrom = dto.NightsFrom,
            NightsTo = dto.NightsTo,
            Budget = dto.Budget,
            Accommodation = dto.Accommodation,
            MealPlan = dto.MealPlan,
            Note = dto.Note,
            DocsPackageDate = dto.DocsPackageDate,
            PrecontractRequired = dto.PrecontractRequired,
            InvoiceRequired = dto.InvoiceRequired,
            CreatedByUserId = "system",
            CreatedAt = DateTime.UtcNow,
            CompanyId = _companyId
        };

        if (dto.LabelIds is { Count: > 0 })
            foreach (var lid in dto.LabelIds.Distinct())
                lead.LeadLabels.Add(new LeadLabel { LabelId = lid, CompanyId = _companyId });

        await uow.Leads.AddAsync(lead, ct);
        await uow.SaveChangesAsync(ct);

        await AddHistoryAsync(lead, "create", actorUserId: lead.CreatedByUserId, ct: ct);
        await uow.SaveChangesAsync(ct);

        return ToDto(lead);
    }

    public async Task UpdateAsync(int id, UpdateLeadDto dto, CancellationToken ct = default)
    {
        var lead = await uow.Leads.GetAsync(id, "system", ct)
                   ?? throw new KeyNotFoundException("Lead not found");

        var before = JsonSerializer.Serialize(ToDto(lead));

        if (dto.ManagerId is > 0)
        {
            var emp = await uow.Users.GetByIdAsync(dto.ManagerId.Value, ct) as Employee
                      ?? throw new InvalidOperationException("Manager must be an employee with office");

            lead.ManagerId = dto.ManagerId;
            lead.OfficeId = emp.OfficeId;

            var mf = string.IsNullOrWhiteSpace(dto.ManagerFullName)
                ? (!string.IsNullOrWhiteSpace(emp.FullName) ? emp.FullName : BuildFullName(emp))
                : dto.ManagerFullName.Trim();

            lead.ManagerFullName = string.IsNullOrWhiteSpace(mf) ? null : mf;
        }
        else
        {
            lead.ManagerId = null;
            lead.ManagerFullName = string.IsNullOrWhiteSpace(dto.ManagerFullName) ? null : dto.ManagerFullName!.Trim();
        }

        lead.LeadStatusId = dto.LeadStatusId;
        lead.RequestTypeId = dto.RequestTypeId;
        lead.SourceId = dto.SourceId;
        lead.CustomerType = dto.CustomerType;
        lead.CustomerFirstName = dto.CustomerFirstName?.Trim() ?? string.Empty;
        lead.CustomerLastName = dto.CustomerLastName?.Trim() ?? string.Empty;
        lead.CustomerMiddleName =
            string.IsNullOrWhiteSpace(dto.CustomerMiddleName) ? null : dto.CustomerMiddleName.Trim();
        lead.Phone = string.IsNullOrWhiteSpace(dto.Phone) ? null : dto.Phone.Trim();
        lead.Email = string.IsNullOrWhiteSpace(dto.Email) ? null : dto.Email.Trim();
        lead.Country = string.IsNullOrWhiteSpace(dto.Country) ? null : dto.Country.Trim();
        lead.Adults = dto.Adults;
        lead.Children = dto.Children;
        lead.Infants = dto.Infants;
        lead.DesiredArrival = dto.DesiredArrival;
        lead.DesiredDeparture = dto.DesiredDeparture;
        lead.NightsFrom = dto.NightsFrom;
        lead.NightsTo = dto.NightsTo;
        lead.Budget = dto.Budget;
        lead.Accommodation = dto.Accommodation;
        lead.MealPlan = dto.MealPlan;
        lead.Note = dto.Note;
        lead.DocsPackageDate = dto.DocsPackageDate;
        lead.PrecontractRequired = dto.PrecontractRequired;
        lead.InvoiceRequired = dto.InvoiceRequired;

        lead.LeadLabels.Clear();
        if (dto.LabelIds is { Count: > 0 })
            foreach (var lid in dto.LabelIds.Distinct())
                lead.LeadLabels.Add(new LeadLabel { LabelId = lid, CompanyId = _companyId });

        lead.UpdatedAt = DateTime.UtcNow;

        uow.Leads.Update(lead);
        await uow.SaveChangesAsync(ct);

        await AddHistoryAsync(lead, "update", actorUserId: lead.CreatedByUserId, beforeSnapshot: before, ct: ct);
        await uow.SaveChangesAsync(ct);
    }

    public async Task AssignUserAsync(int id, int userId, CancellationToken ct = default)
    {
        var lead = await uow.Leads.GetAsync(id, "system", ct)
                   ?? throw new KeyNotFoundException("Lead not found");

        var before = JsonSerializer.Serialize(new { lead.ManagerId, lead.ManagerFullName });

        lead.ManagerId = userId;
        if (await uow.Users.GetByIdAsync(userId, ct) is Employee emp)
        {
            await EnsureOfficeCapacityOrThrowAsync(emp.OfficeId, ct);
            lead.OfficeId = emp.OfficeId;
            lead.ManagerFullName ??= emp.FullName;
        }

        lead.UpdatedAt = DateTime.UtcNow;
        uow.Leads.Update(lead);

        await AddHistoryAsync(lead, "assign", actorUserId: lead.CreatedByUserId, beforeSnapshot: before, ct: ct);
        await uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        await uow.Leads.SoftDeleteAsync(id, userId: "system", ct);
        await AddHistoryAsync(new Lead { Id = id }, "delete", actorUserId: "system", ct: ct);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<LeadDto>> FilterByStatusAsync(string status, CancellationToken ct = default)
    {
        var (items, _) =
            await uow.Leads.SearchAsync(new Core.Specifications.LeadSearchSpec { Page = 1, PageSize = 1000 }, "system",
                ct);
        return items.Where(l => (l.LeadStatus?.Name ?? "").Equals(status, StringComparison.OrdinalIgnoreCase))
            .Select(ToDto);
    }

    public async Task<LeadPageDto> SearchAsync(LeadFilterDto f, CancellationToken ct = default)
    {
        var spec = new Core.Specifications.LeadSearchSpec
        {
            StatusId = f.StatusId,
            CreatedFrom = f.CreatedFrom,
            CreatedTo = f.CreatedTo,
            TravelFrom = f.TravelFrom,
            TravelTo = f.TravelTo,
            NightsMin = f.NightsMin,
            NightsMax = f.NightsMax,
            Country = f.Country,
            BudgetMin = f.BudgetMin,
            BudgetMax = f.BudgetMax,
            ManagerId = f.ManagerId,
            OfficeId = f.OfficeId,
            Query = f.Query,
            SortBy = f.SortBy,
            SortDir = f.SortDir,
            Page = f.Page,
            PageSize = f.PageSize
        };

        var (items, total) = await uow.Leads.SearchAsync(spec, userId: "system", ct);

        return new LeadPageDto
        {
            Items = items.Select(i => new LeadListItemDto
            {
                Id = i.Id,
                LeadNumber = i.LeadNumber,
                Status = i.LeadStatus?.Name ?? string.Empty,
                CreatedAt = i.CreatedAt,
                TravelDate = i.DesiredArrival,
                Nights = i.NightsFrom,
                Country = i.Country,
                Budget = i.Budget,
                Manager = i.ManagerFullName
            }).ToList(),
            Page = f.Page,
            PageSize = f.PageSize,
            Total = total
        };
    }

    public async Task<IReadOnlyList<LeadHistoryDto>> GetHistoryAsync(int leadId, CancellationToken ct = default)
    {
        var items = await uow.LeadHistories.ListAsync(leadId, userId: "system", ct);
        return items.Select(h => new LeadHistoryDto
        {
            CreatedAt = h.CreatedAt,
            Action = h.Action,
            ActorUserId = h.ActorUserId,
            ActorFullName = h.ActorFullName,
            SnapshotJson = h.SnapshotJson
        }).ToList();
    }

    private static string GenerateLeadNumber()
    {
        var year = DateTime.UtcNow.Year;
        var tail = DateTime.UtcNow.Ticks % 1_000_000;
        return $"L-{year}-{tail:D6}";
    }

    private static LeadDto ToDto(Lead l) => new()
    {
        Id = l.Id,
        LeadNumber = l.LeadNumber,
        OfficeId = l.OfficeId,
        LeadStatusId = l.LeadStatusId,
        LeadStatusName = l.LeadStatus?.Name,
        RequestTypeId = l.RequestTypeId,
        RequestTypeName = l.RequestType?.Name,
        SourceId = l.SourceId,
        SourceName = l.Source?.Name,
        ManagerId = l.ManagerId,
        ManagerFullName = l.ManagerFullName,
        CustomerType = l.CustomerType,
        CustomerFirstName = l.CustomerFirstName,
        CustomerLastName = l.CustomerLastName,
        CustomerMiddleName = l.CustomerMiddleName,
        Phone = l.Phone,
        Email = l.Email,
        LabelIds = l.LeadLabels.Select(x => x.LabelId).ToList(),
        Country = l.Country,
        Adults = l.Adults,
        Children = l.Children,
        Infants = l.Infants,
        DesiredArrival = l.DesiredArrival,
        DesiredDeparture = l.DesiredDeparture,
        NightsFrom = l.NightsFrom,
        NightsTo = l.NightsTo,
        Budget = l.Budget,
        Accommodation = l.Accommodation,
        MealPlan = l.MealPlan,
        Note = l.Note,
        DocsPackageDate = l.DocsPackageDate,
        PrecontractRequired = l.PrecontractRequired,
        InvoiceRequired = l.InvoiceRequired,
        CreatedAt = l.CreatedAt
    };

    private async Task AddHistoryAsync(Lead lead, string action, string actorUserId, string? beforeSnapshot = null,
        CancellationToken ct = default)
    {
        if (uow is not { LeadHistories: not null }) return;

        var snapshot = beforeSnapshot ?? JsonSerializer.Serialize(ToDto(lead));

        await uow.LeadHistories.AddAsync(new LeadHistory
        {
            LeadId = lead.Id,
            Action = action,
            ActorUserId = actorUserId,
            ActorFullName = lead.ManagerFullName,
            SnapshotJson = snapshot,
            CompanyId = _companyId
        }, ct);
    }

    private async Task<Employee> PickLeastBusyManagerAsync(CancellationToken ct)
    {
        var candidates = await uow.Users.GetCompanyEmployeesAsync(_companyId, ct);
        if (candidates is null || candidates.Count == 0)
            throw new InvalidOperationException("В компании нет сотрудников для назначения менеджером.");

        var loadByManager = await uow.Leads.GetLeadCountsByManagerAsync(_companyId, ct);
        var loadByOffice = await uow.Leads.GetActiveLeadCountsByOfficeAsync(_companyId, ct);
        var allowed = new List<Employee>();
        foreach (var e in candidates)
        {
            var office = await uow.Offices.GetByIdAsync(e.OfficeId, ct);
            if (office is null) continue;

            var limit = office.LeadLimit;
            if (!(limit is > 0))
            {
                allowed.Add(e);
                continue;
            }

            var current = loadByOffice.GetValueOrDefault(e.OfficeId, 0);
            if (current < limit.Value) allowed.Add(e);
        }

        if (allowed.Count == 0)
            throw new InvalidOperationException("Все офисы достигли лимита лидов.");

        var picked = allowed
            .Select(e => new { Emp = e, Cnt = loadByManager.GetValueOrDefault(e.Id, 0) })
            .OrderBy(x => x.Cnt).ThenBy(x => x.Emp.Id)
            .First().Emp;

        return picked;
    }

    private async Task EnsureOfficeCapacityOrThrowAsync(int officeId, CancellationToken ct)
    {
        var office = await uow.Offices.GetByIdAsync(officeId, ct)
                     ?? throw new InvalidOperationException("Офис не найден");
        var limit = office.LeadLimit;

        if (limit.HasValue && limit.Value > 0)
        {
            var cnt = await uow.Leads.CountActiveByOfficeAsync(_companyId, officeId, ct);
            if (cnt >= limit.Value)
                throw new InvalidOperationException(
                    $"Лимит лидов для офиса «{office.Name}» исчерпан ({cnt}/{limit}).");
        }
    }
}