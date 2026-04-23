using TourCrm.Application.DTOs.Leads;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Entities;
using TourCrm.Core.Entities.Leads;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application.Services;

public sealed class LeadSelectionService(IUnitOfWork uow) : ILeadSelectionService
{
    private static LeadSelectionDto ToDto(LeadSelection e) => new()
    {
        Id = e.Id,
        LeadId = e.LeadId,
        DepartureCity = e.DepartureCity,
        Country = e.Country,
        City = e.City,
        Hotel = e.Hotel,
        RoomType = e.RoomType,
        Accommodation = e.Accommodation,
        MealPlan = e.MealPlan,
        StartDate = e.StartDate,
        Nights = e.Nights,
        Adults = e.Adults,
        Children = e.Children,
        Infants = e.Infants,
        Link = e.Link,
        Note = e.Note,
        PartnerId = e.PartnerId,
        PartnerName = e.PartnerName,
        Price = e.Price,
        Currency = e.Currency,
        CreatedAt = e.CreatedAt
    };

    public async Task<LeadSelectionDto?> GetAsync(int leadId, int id, string userId, CancellationToken ct = default)
    {
        var e = await uow.LeadSelections.GetAsync(leadId, id, ct);
        return e is null ? null : ToDto(e);
    }

    // опционально: удобно, чтобы с /new редиректить на редактирование
    public async Task<LeadSelectionDto?> GetSingleByLeadAsync(int leadId, string userId, CancellationToken ct = default)
    {
        var e = await uow.LeadSelections.GetLastByLeadAsync(leadId, ct);
        return e is null ? null : ToDto(e);
    }

    public async Task<LeadSelectionDto> CreateAsync(int leadId, CreateLeadSelectionDto dto, string userId, CancellationToken ct = default)
    {
        var lead = await uow.Leads.GetByIdAsync(leadId, ct)
                   ?? throw new KeyNotFoundException("Lead not found");

        // ИДЕМПОТЕНТНО: если у лида уже есть подборка — обновим её, а не создадим новую
        var existing = await uow.LeadSelections.GetByLeadForUpdateAsync(leadId, ct);
        if (existing is not null)
        {
            await MapFromCreateDtoAsync(existing, dto, ct);
            existing.UpdatedAt = DateTime.UtcNow;
            await uow.SaveChangesAsync(ct);
            return ToDto(existing);
        }

        // создать новую
        var sel = new LeadSelection
        {
            LeadId = leadId,
            CompanyId = lead.CompanyId,
            CreatedByUserId = string.IsNullOrEmpty(userId) ? "system" : userId,
            CreatedAt = DateTime.UtcNow
        };

        await MapFromCreateDtoAsync(sel, dto, ct);
        await uow.LeadSelections.AddAsync(sel, ct);
        await uow.SaveChangesAsync(ct);
        return ToDto(sel);
    }

    public async Task<LeadSelectionDto> UpdateAsync(int leadId, int id, UpdateLeadSelectionDto dto, string userId, CancellationToken ct = default)
    {
        _ = await uow.Leads.GetByIdAsync(leadId, ct)
            ?? throw new KeyNotFoundException("Lead not found");

        var sel = await uow.LeadSelections.GetForUpdateAsync(leadId, id, ct)
                  ?? throw new KeyNotFoundException("Selection not found");

        await MapFromUpdateDtoAsync(sel, dto, ct);
        sel.UpdatedAt = DateTime.UtcNow;
        uow.LeadSelections.Update(sel);
        await uow.SaveChangesAsync(ct);
        return ToDto(sel);
    }

    // -------- маппинг без дополнительных интерфейсов --------

    private async Task MapFromCreateDtoAsync(LeadSelection sel, CreateLeadSelectionDto dto, CancellationToken ct)
    {
        sel.DepartureCity = dto.DepartureCity.Trim();
        sel.Country       = dto.Country.Trim();
        sel.City          = dto.City.Trim();
        sel.Hotel         = string.IsNullOrWhiteSpace(dto.Hotel) ? null : dto.Hotel.Trim();
        sel.RoomType      = string.IsNullOrWhiteSpace(dto.RoomType) ? null : dto.RoomType.Trim();
        sel.Accommodation = dto.Accommodation.Trim();
        sel.MealPlan      = dto.MealPlan.Trim();
        sel.StartDate     = dto.StartDate;
        sel.Nights        = dto.Nights;
        sel.Adults        = dto.Adults;
        sel.Children      = dto.Children;
        sel.Infants       = dto.Infants;
        sel.Link          = string.IsNullOrWhiteSpace(dto.Link) ? null : dto.Link.Trim();
        sel.Note          = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim();
        sel.Price         = dto.Price;
        sel.Currency      = string.IsNullOrWhiteSpace(dto.Currency) ? "RUB" : dto.Currency!.Trim();

        sel.PartnerId     = dto.PartnerId;
        sel.PartnerName   = null;
        if (dto.PartnerId is { } pid && pid > 0)
        {
            var p = await uow.Partners.GetByIdAsync(pid, ct);
            sel.PartnerName = p?.Name ?? dto.PartnerName?.Trim();
        }
        else if (!string.IsNullOrWhiteSpace(dto.PartnerName))
        {
            sel.PartnerName = dto.PartnerName.Trim();
        }
    }

    private async Task MapFromUpdateDtoAsync(LeadSelection sel, UpdateLeadSelectionDto dto, CancellationToken ct)
    {
        sel.DepartureCity = dto.DepartureCity.Trim();
        sel.Country       = dto.Country.Trim();
        sel.City          = dto.City.Trim();
        sel.Hotel         = string.IsNullOrWhiteSpace(dto.Hotel) ? null : dto.Hotel.Trim();
        sel.RoomType      = string.IsNullOrWhiteSpace(dto.RoomType) ? null : dto.RoomType.Trim();
        sel.Accommodation = dto.Accommodation.Trim();
        sel.MealPlan      = dto.MealPlan.Trim();
        sel.StartDate     = dto.StartDate;
        sel.Nights        = dto.Nights;
        sel.Adults        = dto.Adults;
        sel.Children      = dto.Children;
        sel.Infants       = dto.Infants;
        sel.Link          = string.IsNullOrWhiteSpace(dto.Link) ? null : dto.Link.Trim();
        sel.Note          = string.IsNullOrWhiteSpace(dto.Note) ? null : dto.Note.Trim();
        sel.Price         = dto.Price;
        sel.Currency      = string.IsNullOrWhiteSpace(dto.Currency) ? "RUB" : dto.Currency!.Trim();

        sel.PartnerId     = dto.PartnerId;
        sel.PartnerName   = null;
        if (dto.PartnerId is { } pid && pid > 0)
        {
            var p = await uow.Partners.GetByIdAsync(pid, ct);
            sel.PartnerName = p?.Name ?? dto.PartnerName?.Trim();
        }
        else if (!string.IsNullOrWhiteSpace(dto.PartnerName))
        {
            sel.PartnerName = dto.PartnerName.Trim();
        }
    }
}