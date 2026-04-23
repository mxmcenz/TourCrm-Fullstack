using TourCrm.Core.Interfaces;
using TourCrm.Core.Interfaces.Dictionaries;
using TourCrm.Infrastructure.Data;

namespace TourCrm.Infrastructure.Repositories;

public class UnitOfWork(
    TourCrmDbContext context,
    IUserRepository users,
    ILeadRepository leads,
    IRefreshTokenRepository refreshTokens,
    IDealRepository deals,
    ICountryRepository countries,
    IRoleRepository roleRepository,
    IRolePermissionRepository rolePermissionRepository,
    IUserRoleRepository userRoleRepository,
    IEmployeeRepository employees,
    ICityRepository cities,
    IServiceTypeRepository serviceTypes,
    INumberTypeRepository numberTypes,
    IAccommodationTypeRepository accommodationTypes,
    IMealTypeRepository mealTypes,
    IPartnerRepository partners,
    ITourOperatorRepository tourOperator,
    ICitizenshipRepository citizenships,
    IPartnerMarkRepository partnerMarks,
    IPartnerTypeRepository partnerTypes,
    IClientRepository clientRepository,
    IAuditLogRepository auditLogRepository,
    ILeadSelectionRepository leadSelectionRepository,
    ILeadHistoryRepository leadHistoryRepository,
    ICompanyRepository companyRepository,
    IDealStatusRepository dealStatusRepository,
    IDealHistoryRepository dealHistoryRepository,
    IOfficeRepository officeRepository,
    ITariffRepository tariffRepository,
    ITariffPermissionRepository tariffPermissionRepository,
    ILegalEntityRepository legalEntities) : IUnitOfWork
{
    public IUserRepository Users { get; } = users;
    public ILeadRepository Leads { get; } = leads;
    public ILeadSelectionRepository LeadSelections { get; } = leadSelectionRepository;
    public ILeadHistoryRepository LeadHistories { get; } = leadHistoryRepository;
    public IRefreshTokenRepository RefreshTokens { get; } = refreshTokens;
    public IDealRepository Deals { get; } = deals;
    public ICompanyRepository Companies { get; } = companyRepository;
    public IDealStatusRepository DealStatuses { get; } = dealStatusRepository;
    public IDealHistoryRepository DealHistories { get; } = dealHistoryRepository;
    public ICountryRepository Countries { get; } = countries;
    public IRoleRepository Roles { get; } = roleRepository;
    public IRolePermissionRepository RolePermissions { get; } = rolePermissionRepository;
    public IUserRoleRepository UserRoles { get; } = userRoleRepository;
    public ICityRepository Cities { get; } = cities;
    public IEmployeeRepository Employees { get; } = employees;
    public IServiceTypeRepository ServiceTypes { get; set; } = serviceTypes;
    public INumberTypeRepository NumberTypes { get; } = numberTypes;
    public IAccommodationTypeRepository AccommodationTypes { get; } = accommodationTypes;
    public IMealTypeRepository MealTypes { get; } = mealTypes;
    public IPartnerRepository Partners { get; } = partners;
    public ITourOperatorRepository TourOperators { get; } = tourOperator;
    public ICitizenshipRepository Citizenships { get; } = citizenships;
    public IPartnerMarkRepository PartnerMarks { get; } = partnerMarks;
    public IPartnerTypeRepository PartnerTypes { get; set; } = partnerTypes;
    public IClientRepository Clients { get; } = clientRepository;
    public IAuditLogRepository AuditLogs { get; } = auditLogRepository;
    public IOfficeRepository Offices { get; } = officeRepository;
    public ITariffRepository Tariffs { get; } = tariffRepository;
    public ITariffPermissionRepository TariffPermissions { get; } = tariffPermissionRepository;
    public ILegalEntityRepository LegalEntities { get; } = legalEntities;

    public Task<int> SaveChangesAsync(CancellationToken ct = default)
        => context.SaveChangesAsync(ct);
}