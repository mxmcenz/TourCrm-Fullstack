using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Interfaces;
using TourCrm.Core.Interfaces.Dictionaries;
using TourCrm.Infrastructure.Data;
using TourCrm.Infrastructure.Repositories;
using TourCrm.Infrastructure.Repositories.Dictionaries;
using TourCrm.Infrastructure.Services;

namespace TourCrm.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<TourCrmDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ILeadRepository, LeadRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        services.AddScoped<IRolePermissionRepository, RolePermissionRepository>();
        services.AddScoped<IUserRoleRepository, UserRoleRepository>();
        services.AddScoped<IEmployeeRepository, EmployeeRepository>();
        services.AddScoped<IPasswordHasher, BCryptPasswordHasher>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IReferenceDataSeeder, ReferenceDataSeeder>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IDealRepository, DealRepository>();
        services.AddScoped<IDealStatusRepository, DealStatusRepository>();
        services.AddScoped<IDealHistoryRepository, DealHistoryRepository>();
        services.AddScoped<ICountryRepository, CountryRepository>();
        services.AddScoped<ILegalEntityRepository, LegalEntityRepository>();
        services.AddScoped<IOfficeRepository, OfficeRepository>();
        services.AddScoped<ICompanyRepository, CompanyRepository>();
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();
        services.AddScoped<INumberTypeRepository, NumberTypeRepository>();
        services.AddScoped<IAccommodationTypeRepository, AccommodationTypeRepository>();
        services.AddScoped<IMealTypeRepository, MealTypeRepository>();
        services.AddScoped<IPartnerRepository, PartnerRepository>();
        services.AddScoped<ITourOperatorRepository, TourOperatorRepository>();
        services.AddScoped<ICitizenshipRepository, CitizenshipRepository>();
        services.AddScoped<IPartnerMarkRepository, PartnerMarkRepository>();
        services.AddScoped<IPartnerTypeRepository, PartnerTypeRepository>();
        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<IAuditLogRepository, AuditLogRepository>();
        services.AddScoped<ILeadSelectionRepository, LeadSelectionRepository>();
        services.AddScoped<ILeadHistoryRepository, LeadHistoryRepository>();
        services.AddScoped<ILeadStatusRepository, LeadStatusRepository>();
        services.AddScoped<ILeadSourceRepository, LeadSourceRepository>();
        services.AddScoped<ILeadRequestTypeRepository, LeadRequestTypeRepository>();
        services.AddScoped<ILabelRepository, LabelRepository>();
        services.AddScoped<IHotelRepository, HotelRepository>();
        services.AddScoped<ICurrencyRepository, CurrencyRepository>();
        services.AddScoped<IVisaTypeRepository, VisaTypeRepository>();
        services.AddScoped<ITariffRepository, TariffRepository>();
        services.AddScoped<ITariffPermissionRepository, TariffPermissionRepository>();
        return services;
    }
}