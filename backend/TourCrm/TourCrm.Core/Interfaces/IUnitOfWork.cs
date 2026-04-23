using TourCrm.Core.Interfaces.Dictionaries;

namespace TourCrm.Core.Interfaces;

public interface IUnitOfWork
{
    IUserRepository Users { get; }
    ILeadRepository Leads { get; }
    ILeadSelectionRepository LeadSelections { get; }
    ILeadHistoryRepository LeadHistories { get; }
    IRefreshTokenRepository RefreshTokens { get; }
    IDealRepository Deals { get; }
    IDealStatusRepository DealStatuses { get; }
    IDealHistoryRepository DealHistories { get; }
    ICountryRepository Countries { get; }
    IRoleRepository Roles { get; }
    IEmployeeRepository Employees { get; }
    IRolePermissionRepository RolePermissions { get; }
    IUserRoleRepository UserRoles { get; }
    ICityRepository Cities { get; }
    IServiceTypeRepository ServiceTypes { get; }
    INumberTypeRepository NumberTypes { get; }
    IAccommodationTypeRepository AccommodationTypes { get; }
    IMealTypeRepository MealTypes { get; }
    IPartnerRepository Partners { get; }
    ITourOperatorRepository TourOperators { get; }
    ICitizenshipRepository Citizenships { get; }
    IPartnerMarkRepository PartnerMarks { get; }
    IClientRepository Clients { get; }
    IAuditLogRepository AuditLogs { get; }
    IPartnerTypeRepository PartnerTypes { get; }
    ICompanyRepository Companies { get; }
    IOfficeRepository Offices { get; }
    ITariffRepository Tariffs { get; }
    ITariffPermissionRepository TariffPermissions { get; }
    ILegalEntityRepository LegalEntities { get; }
    Task<int> SaveChangesAsync(CancellationToken ct = default);
}