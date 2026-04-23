using TourCrm.Application.DTOs.Deals;
using TourCrm.Application.DTOs.Deals.FiledsForCreate;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Abstractions;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Client;
using TourCrm.Core.Entities.Deals;
using TourCrm.Core.Enums;
using TourCrm.Core.Interfaces;
using TourCrm.Core.Specifications;

namespace TourCrm.Application.Services;

public class DealService(IUnitOfWork uow, ICompanyContext companyContext) : IDealService
{
    private readonly int _companyId = companyContext.CompanyId;

    public async Task<List<DealDto>> GetAllAsync(
        int? statusId = null, int? officeId = null, int? managerId = null, CancellationToken ct = default)
        => (await uow.Deals.GetAllWithIncludesAsync(statusId, officeId, managerId, ct))
           .Where(d => d.CompanyId == _companyId)
           .Select(ToDto)
           .ToList();

    public async Task<DealDto?> GetAsync(int id, CancellationToken ct = default)
    {
        var d = await uow.Deals.GetByIdWithIncludesAsync(id, ct);
        return d is not null && d.CompanyId == _companyId ? ToDto(d) : null;
    }

    public async Task<DealDto> CreateAsync(CreateDealDto dto, CancellationToken ct = default)
    {
        var statusId = await ResolveStatusIdAsync(dto.StatusId, ct);

        var deal = new Deal
        {
            DealNumber = string.IsNullOrWhiteSpace(dto.DealNumber) ? GenerateDealNumber() : dto.DealNumber!,
            InternalNumber = dto.InternalNumber,
            StatusId = statusId,
            LeadId = dto.LeadId,
            ManagerId = dto.ManagerId,
            CompanyId = _companyId, 
            IssuerLegalEntityId = dto.IssuerLegalEntityId,
            RequestTypeId = dto.RequestTypeId,
            SourceId = dto.SourceId,
            TourOperatorId = dto.TourOperatorId,
            VisaTypeId = dto.VisaTypeId,
            TourName = dto.TourName,
            Price = dto.Price,
            BookingNumbers = dto.BookingNumbers,
            Note = dto.Note,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            ClientPaymentDeadline = dto.ClientPaymentDeadline,
            PartnerPaymentDeadline = dto.PartnerPaymentDeadline,
            DocsPackageDate = dto.DocsPackageDate,
            AddStampAndSign = dto.AddStampAndSign,
            IncludeCostCalc = dto.IncludeCostCalc,
            IncludeTourCalc = dto.IncludeTourCalc,
            CreatedByUserId = "system",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await uow.Deals.AddAsync(deal, ct);
        await uow.SaveChangesAsync(ct);

        var cache = new Dictionary<int, Client>();

        if (dto.Customers is { Count: > 0 })
        {
            var seen = new HashSet<int>();
            foreach (var c in dto.Customers)
            {
                if (c.ClientId is int cid)
                {
                    if (!seen.Add(cid)) continue;
                    var client = await GetClientOnceAsync(deal.CompanyId, cid, cache, ct);
                    deal.Customers.Add(client);
                }
                else
                {
                    var client = await EnsureClientAsync(c.FullName ?? "", c.Phone, c.Email,
                        isTourist: false, companyId: deal.CompanyId, ct);
                    deal.Customers.Add(client);
                }
            }
        }

        if (dto.Tourists is { Count: > 0 })
        {
            var seen = new HashSet<int>();
            foreach (var t in dto.Tourists)
            {
                if (t.ClientId is int cid)
                {
                    if (!seen.Add(cid)) continue;
                    var client = await GetClientOnceAsync(deal.CompanyId, cid, cache, ct);
                    if (!client.IsTourist) { client.IsTourist = true; uow.Clients.Update(client); }
                    deal.Tourists.Add(client);
                }
                else
                {
                    var client = await EnsureClientAsync(t.FullName ?? "", t.Phone, t.Email,
                        isTourist: true, companyId: deal.CompanyId, ct);
                    deal.Tourists.Add(client);
                }
            }
        }

        if (dto.Services is { Count: > 0 })
        {
            foreach (var s in dto.Services)
                deal.Services.Add(new DealServiceItem
                {
                    Name = s.Name.Trim(),
                    Note = string.IsNullOrWhiteSpace(s.Note) ? null : s.Note.Trim(),
                });
        }

        if (dto.ClientPays is { Count: > 0 })
        {
            foreach (var p in dto.ClientPays)
                deal.ClientPayments.Add(new DealClientPayment { Title = p.Title.Trim(), Amount = p.Amount });
        }

        if (dto.PartnerPays is { Count: > 0 })
        {
            foreach (var p in dto.PartnerPays)
                deal.PartnerPayments.Add(new DealPartnerPayment { Title = p.Title.Trim(), Amount = p.Amount });
        }

        await uow.SaveChangesAsync(ct);
        await AddHistoryAsync(deal.Id, deal.CompanyId, "created", $"Создана сделка {deal.DealNumber}", ct);
        await uow.SaveChangesAsync(ct);

        var created = await uow.Deals.GetByIdWithIncludesAsync(deal.Id, ct) ?? deal;
        return ToDto(created);
    }

    public async Task UpdateAsync(int id, UpdateDealDto dto, CancellationToken ct = default)
    {
        var d = await uow.Deals.GetByIdWithIncludesAsync(id, ct)
                ?? throw new KeyNotFoundException("Deal not found");

        if (d.CompanyId != _companyId) 
            throw new KeyNotFoundException("Deal not found");

        if (dto.DealNumber is not null) d.DealNumber = dto.DealNumber;
        if (dto.InternalNumber is not null) d.InternalNumber = dto.InternalNumber;
        if (dto.StatusId.HasValue) d.StatusId = await ResolveStatusIdAsync(dto.StatusId, ct);
        if (dto.LeadId.HasValue) d.LeadId = dto.LeadId;
        if (dto.ManagerId.HasValue) d.ManagerId = dto.ManagerId.Value;
        if (dto.IssuerLegalEntityId.HasValue) d.IssuerLegalEntityId = dto.IssuerLegalEntityId.Value;
        if (dto.RequestTypeId.HasValue) d.RequestTypeId = dto.RequestTypeId.Value;
        if (dto.SourceId.HasValue) d.SourceId = dto.SourceId.Value;
        if (dto.TourOperatorId.HasValue) d.TourOperatorId = dto.TourOperatorId.Value;
        if (dto.VisaTypeId.HasValue) d.VisaTypeId = dto.VisaTypeId.Value;
        if (dto.TourName is not null) d.TourName = dto.TourName;
        if (dto.Price.HasValue) d.Price = dto.Price.Value;
        if (dto.BookingNumbers is not null) d.BookingNumbers = dto.BookingNumbers;
        if (dto.Note is not null) d.Note = dto.Note;
        if (dto.StartDate.HasValue) d.StartDate = dto.StartDate;
        if (dto.EndDate.HasValue) d.EndDate = dto.EndDate;
        if (dto.ClientPaymentDeadline.HasValue) d.ClientPaymentDeadline = dto.ClientPaymentDeadline;
        if (dto.PartnerPaymentDeadline.HasValue) d.PartnerPaymentDeadline = dto.PartnerPaymentDeadline;
        if (dto.DocsPackageDate.HasValue) d.DocsPackageDate = dto.DocsPackageDate;
        if (dto.AddStampAndSign.HasValue) d.AddStampAndSign = dto.AddStampAndSign.Value;
        if (dto.IncludeCostCalc.HasValue) d.IncludeCostCalc = dto.IncludeCostCalc.Value;
        if (dto.IncludeTourCalc.HasValue) d.IncludeTourCalc = dto.IncludeTourCalc.Value;

        var tracked = d.Customers.Concat(d.Tourists).GroupBy(c => c.Id).ToDictionary(g => g.Key, g => g.First());
        var cache = new Dictionary<int, Client>(tracked);

        if (dto.Customers is not null || dto.Tourists is not null)
        {
            d.Customers.Clear();
            d.Tourists.Clear();
        }

        if (dto.Customers is not null)
        {
            var seenIds = new HashSet<int>();
            foreach (var c in dto.Customers)
            {
                if (c.ClientId is int cid)
                {
                    if (!seenIds.Add(cid)) continue;
                    var client = await GetClientOnceAsync(d.CompanyId, cid, cache, ct);
                    d.Customers.Add(client);
                }
                else
                {
                    var client = await EnsureClientAsync(c.FullName ?? "", c.Phone, c.Email, false, d.CompanyId, ct);
                    d.Customers.Add(client);
                }
            }
        }

        if (dto.Tourists is not null)
        {
            var seenIds = new HashSet<int>();
            foreach (var t in dto.Tourists)
            {
                if (t.ClientId is int cid)
                {
                    if (!seenIds.Add(cid)) continue;
                    var client = await GetClientOnceAsync(d.CompanyId, cid, cache, ct);
                    if (!client.IsTourist) { client.IsTourist = true; uow.Clients.Update(client); }
                    d.Tourists.Add(client);
                }
                else
                {
                    var client = await EnsureClientAsync(t.FullName ?? "", t.Phone, t.Email, true, d.CompanyId, ct);
                    d.Tourists.Add(client);
                }
            }
        }

        if (dto.Services is not null)
        {
            d.Services.Clear();
            foreach (var s in dto.Services)
                d.Services.Add(new DealServiceItem { Name = s.Name.Trim(), Note = string.IsNullOrWhiteSpace(s.Note) ? null : s.Note.Trim() });
        }

        if (dto.ClientPays is not null)
        {
            d.ClientPayments.Clear();
            foreach (var p in dto.ClientPays)
                d.ClientPayments.Add(new DealClientPayment { Title = p.Title.Trim(), Amount = p.Amount });
        }

        if (dto.PartnerPays is not null)
        {
            d.PartnerPayments.Clear();
            foreach (var p in dto.PartnerPays)
                d.PartnerPayments.Add(new DealPartnerPayment { Title = p.Title.Trim(), Amount = p.Amount });
        }

        d.UpdatedAt = DateTimeOffset.UtcNow;
        uow.Deals.Update(d);
        await uow.SaveChangesAsync(ct);
        await AddHistoryAsync(d.Id, d.CompanyId, "updated", "Сделка обновлена", ct);
        await uow.SaveChangesAsync(ct);
    }

    public async Task ChangeStatusAsync(int id, int statusId, CancellationToken ct = default)
    {
        var deal = await uow.Deals.GetByIdWithIncludesAsync(id, ct)
                   ?? throw new KeyNotFoundException("Deal not found");
        if (deal.CompanyId != _companyId) throw new KeyNotFoundException("Deal not found");
        var newStatus = await uow.DealStatuses.GetByIdAsync(statusId, ct)
                        ?? throw new KeyNotFoundException("Status not found");
        var oldStatusName = deal.Status?.Name ?? deal.StatusId.ToString();
        await uow.Deals.ChangeStatusAsync(id, statusId, ct);
        await uow.SaveChangesAsync(ct);
        var note = $"Статус: {oldStatusName} → {newStatus.Name}";
        await AddHistoryAsync(deal.Id, deal.CompanyId, "status_changed", note, ct);
        await uow.SaveChangesAsync(ct);
    }

    public async Task<DealDto> CreateFromLeadAsync(int leadId, int managerId, int touristId, CancellationToken ct = default)
    {
        var lead = await uow.Leads.GetAsync(leadId, userId: "system", ct)
                   ?? throw new KeyNotFoundException("Lead not found");
        if (lead.CompanyId != _companyId)
            throw new InvalidOperationException("Lead belongs to another company");

        var statusId = (await uow.DealStatuses.GetDefaultAsync(ct)
                        ?? throw new InvalidOperationException("Default deal status is not configured")).Id;

        var deal = new Deal
        {
            DealNumber = GenerateDealNumber(),
            StatusId = statusId,
            LeadId = lead.Id,
            ManagerId = managerId,
            CompanyId = _companyId,
            TourName = !string.IsNullOrWhiteSpace(lead.Country) ? $"Тур в {lead.Country}" : "Тур",
            Price = lead.Budget ?? 0m,
            StartDate = lead.DesiredArrival,
            EndDate   = lead.DesiredDeparture,
            CreatedByUserId = "system",
            CreatedAt = DateTimeOffset.UtcNow
        };

        await uow.Deals.AddAsync(deal, ct);
        await uow.SaveChangesAsync(ct);

        var tourist = await uow.Clients.GetByIdAsync(deal.CompanyId, touristId, ct)
                      ?? throw new KeyNotFoundException("Tourist client not found");
        if (!tourist.IsTourist) { tourist.IsTourist = true; uow.Clients.Update(tourist); }

        deal.Tourists.Add(tourist);
        await uow.SaveChangesAsync(ct);
        await AddHistoryAsync(deal.Id, deal.CompanyId, "created_from_lead", $"Создано из лида #{lead.Id}", ct);
        await uow.SaveChangesAsync(ct);
        var created = await uow.Deals.GetByIdWithIncludesAsync(deal.Id, ct) ?? deal;
        return ToDto(created);
    }

    public async Task<List<DealHistoryDto>> GetHistoryAsync(int dealId, CancellationToken ct = default)
    {
        var d = await uow.Deals.GetByIdIgnoringFiltersAsync(dealId, ct);
        if (d is null || d.CompanyId != _companyId) throw new KeyNotFoundException("Deal not found");

        var items = await uow.DealHistories.GetByDealAsync(dealId, ct);
        return items.Select(h => new DealHistoryDto
        {
            Id = h.Id, Action = h.Action, Note = h.Note,
            ActorUserId = h.ActorUserId, ActorFullName = h.ActorFullName,
            CreatedAt = h.CreatedAt
        }).ToList();
    }

    public async Task<PagedResult<DealDto>> SearchAsync(DealSearchRequestDto dto, CancellationToken ct = default)
    {
        var req = new DealSearchRequest
        {
            Page = dto.Page, PageSize = dto.PageSize, Query = dto.Query,
            Scope = dto.Scope, SortBy = dto.SortBy, SortDir = dto.SortDir,
            DealNumber = dto.DealNumber, Booking = dto.Booking,
            StatusId = dto.StatusId, ManagerId = dto.ManagerId,
            CreatedFrom = dto.CreatedFrom, CreatedTo = dto.CreatedTo,
            StartFrom = dto.StartFrom, StartTo = dto.StartTo,
            EndFrom = dto.EndFrom, EndTo = dto.EndTo,
            ClientPaymentFrom = dto.ClientPaymentFrom, ClientPaymentTo = dto.ClientPaymentTo,
            PartnerPaymentFrom = dto.PartnerPaymentFrom, PartnerPaymentTo = dto.PartnerPaymentTo,
            Tourist = dto.Tourist, PriceFrom = dto.PriceFrom, PriceTo = dto.PriceTo,
            CompanyId = _companyId
        };

        var res = await uow.Deals.SearchAsync(req, ct);

        var items = res.Items.Where(d => d.CompanyId == _companyId).Select(ToDto).ToList();
        return new PagedResult<DealDto>(items, res.Total);
    }

    public async Task ArchiveAsync(int id, CancellationToken ct = default)
    {
        var d = await uow.Deals.GetByIdWithIncludesAsync(id, ct)
                ?? throw new KeyNotFoundException("Deal not found");
        if (d.CompanyId != _companyId) throw new KeyNotFoundException("Deal not found");

        if (!d.IsDeleted)
        {
            d.IsDeleted = true;
            d.UpdatedAt = DateTimeOffset.UtcNow;
            uow.Deals.Update(d);
            await uow.SaveChangesAsync(ct);
            await AddHistoryAsync(d.Id, d.CompanyId, "archived", "Сделка перемещена в архив", ct);
            await uow.SaveChangesAsync(ct);
        }
    }

    public async Task RestoreAsync(int id, CancellationToken ct = default)
    {
        var d = await uow.Deals.GetByIdIgnoringFiltersAsync(id, ct)
                ?? throw new KeyNotFoundException("Deal not found");
        if (d.CompanyId != _companyId) throw new KeyNotFoundException("Deal not found");

        if (d.IsDeleted)
        {
            d.IsDeleted = false;
            d.UpdatedAt = DateTimeOffset.UtcNow;
            uow.Deals.Update(d);
            await uow.SaveChangesAsync(ct);
            await AddHistoryAsync(d.Id, d.CompanyId, "restored", "Сделка восстановлена из архива", ct);
            await uow.SaveChangesAsync(ct);
        }
    }

    private async Task AddHistoryAsync(int dealId, int companyId, string action, string? note, CancellationToken ct)
    {
        var history = new DealHistory
        {
            DealId = dealId, CompanyId = companyId, Action = action, Note = note,
            ActorUserId = "system", ActorFullName = null, CreatedAt = DateTime.UtcNow
        };
        await uow.DealHistories.AddAsync(history, ct);
    }

    private async Task<int> ResolveStatusIdAsync(int? id, CancellationToken ct)
    {
        if (id.HasValue)
            return (await uow.DealStatuses.GetByIdAsync(id.Value, ct)
                    ?? throw new KeyNotFoundException("Status not found")).Id;

        return (await uow.DealStatuses.GetDefaultAsync(ct)
                ?? throw new InvalidOperationException("Default deal status is not configured")).Id;
    }

    private static string GenerateDealNumber() => $"D-{DateTime.UtcNow:yyyyMMddHHmmss}";

    private static DealDto ToDto(Deal d)
    {
        var lastSel = d.Lead?.Selections != null && d.Lead.Selections.Any()
            ? d.Lead.Selections.OrderByDescending(s => s.Id).FirstOrDefault()
            : null;

        return new DealDto
        {
            Id = d.Id,
            DealNumber = d.DealNumber,
            InternalNumber = d.InternalNumber,
            StatusId = d.StatusId,
            StatusName = d.Status?.Name ?? "",
            LeadId = d.LeadId,
            ManagerId = d.ManagerId,
            ManagerName = d.Manager?.FullName ?? "",
            CompanyId = d.CompanyId,
            CompanyName = d.Company?.Name ?? "",
            IssuerLegalEntityId = d.IssuerLegalEntityId,
            IssuerLegalEntityName = d.IssuerLegalEntity?.Name,
            RequestTypeId = d.RequestTypeId,
            RequestTypeName = d.RequestType?.Name,
            SourceId = d.SourceId,
            SourceName = d.Source?.Name,
            TourOperatorId = d.TourOperatorId,
            TourOperatorName = d.TourOperator?.Name,
            VisaTypeId = d.VisaTypeId,
            TourName = d.TourName,
            Price = d.Price,
            BookingNumbers = d.BookingNumbers,
            Note = d.Note,
            StartDate = d.StartDate,
            EndDate = d.EndDate,
            ClientPaymentDeadline = d.ClientPaymentDeadline,
            PartnerPaymentDeadline = d.PartnerPaymentDeadline,
            DocsPackageDate = d.DocsPackageDate,
            AddStampAndSign = d.AddStampAndSign,
            IncludeCostCalc = d.IncludeCostCalc,
            IncludeTourCalc = d.IncludeTourCalc,
            CreatedAt = d.CreatedAt,
            LeadCreatedAt = d.Lead?.CreatedAt,
            CountryName   = lastSel?.Country ?? d.Lead?.Country,
            CityName      = lastSel?.City,
            HotelName     = lastSel?.Hotel,
            RoomType      = lastSel?.RoomType,
            AccommodationType = lastSel?.Accommodation,
            BookingLink   = lastSel?.Link,

            Customers = d.Customers.Select(c => new ClientShortDto
            {
                Id = c.Id,
                FullName = BuildFullName(c),
                Phone = c.PhoneE164,
                Email = c.Email
            }).ToList(),

            Tourists = d.Tourists.Select(c => new ClientShortDto
            {
                Id = c.Id,
                FullName = BuildFullName(c),
                Phone = c.PhoneE164,
                Email = c.Email
            }).ToList(),

            Services   = d.Services.Select(s => new NewServiceDto(s.Name, s.Note)).ToList(),
            ClientPays = d.ClientPayments.Select(p => new NewPaymentDto(p.Title, p.Amount)).ToList(),
            PartnerPays= d.PartnerPayments.Select(p => new NewPaymentDto(p.Title, p.Amount)).ToList(),
        };
    }

    private async Task<Client> GetClientOnceAsync(int companyId, int clientId, IDictionary<int, Client> cache, CancellationToken ct)
    {
        if (cache.TryGetValue(clientId, out var cached)) return cached;
        var client = await uow.Clients.GetByIdAsync(companyId, clientId, ct)
                     ?? throw new KeyNotFoundException("Client not found");
        cache[clientId] = client;
        return client;
    }

    private static string BuildFullName(Client c)
    {
        var p = new[] { c.LastName, c.FirstName, c.MiddleName }.Where(s => !string.IsNullOrWhiteSpace(s));
        return string.Join(' ', p);
    }

    private async Task<Client> EnsureClientAsync(
        string fullName, string? phone, string? email, bool isTourist, int companyId, CancellationToken ct)
    {
        Client? found = null;

        if (!string.IsNullOrWhiteSpace(email))
        {
            var e = email.Trim();
            var byEmail = await uow.Clients.SearchAsync(companyId, e, page: 1, pageSize: 10, ct);
            found = byEmail.FirstOrDefault(c => string.Equals(c.Email, e, StringComparison.OrdinalIgnoreCase));
        }

        if (found is null && !string.IsNullOrWhiteSpace(phone))
        {
            var p = phone.Trim();
            var byPhone = await uow.Clients.SearchAsync(companyId, p, page: 1, pageSize: 10, ct);
            found = byPhone.FirstOrDefault(c => c.PhoneE164 == p);
        }

        if (found is not null)
        {
            if (isTourist && !found.IsTourist) { found.IsTourist = true; uow.Clients.Update(found); }
            return found;
        }

        var (last, first, middle) = SplitFullName(fullName);
        var client = new Client
        {
            CompanyId = companyId,
            ClientType = ClientType.PrivatePerson,
            FirstName = first, LastName = last, MiddleName = middle,
            PhoneE164 = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim(),
            Email = string.IsNullOrWhiteSpace(email) ? null : email.Trim(),
            IsTourist = isTourist
        };

        await uow.Clients.AddAsync(client, ct);
        return client;
    }

    private static (string last, string first, string middle) SplitFullName(string full)
    {
        var parts = (full ?? "").Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var first = parts.ElementAtOrDefault(1) ?? parts.ElementAtOrDefault(0) ?? "";
        var last  = parts.ElementAtOrDefault(0) ?? "";
        var middle = parts.Length > 2 ? string.Join(' ', parts.Skip(2)) : "";
        return (last, first, middle);
    }
}