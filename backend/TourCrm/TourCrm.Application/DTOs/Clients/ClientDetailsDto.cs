using TourCrm.Core.Enums;

namespace TourCrm.Application.DTOs.Clients;

public sealed record ClientDetailsDto : ClientListItemDto
{
    public int CompanyId { get; init; }
    public ClientType ClientType { get; init; }
    public int? ManagerId { get; init; }
    public string? MiddleName { get; init; }
    public string? FirstNameGenitive { get; init; }
    public string? LastNameGenitive { get; init; }
    public string? MiddleNameGenitive { get; init; }
    public DateOnly? BirthDay { get; init; }
    public Gender Gender { get; init; }
    public bool IsSubscribedToMailing { get; init; }
    public bool IsEmailNotificationEnabled { get; init; }
    public string? ReferredBy { get; init; }
    public string? Note { get; init; }
    public decimal DiscountPercent { get; init; }
}