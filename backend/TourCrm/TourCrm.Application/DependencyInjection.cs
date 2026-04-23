using Microsoft.Extensions.DependencyInjection;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.Application.Services;
using TourCrm.Application.Services.Dictionaries;
using TourCrm.Core.Interfaces;

namespace TourCrm.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddScoped<ILeadService, LeadService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IDealService, DealService>();
        services.AddScoped<IDealStatusService, DealStatusService>();
        services.AddScoped<ILegalEntityService, LegalEntityService>();
        services.AddScoped<IOfficeService, OfficeService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<IEmployeeService, EmployeeService>();
        services.AddScoped<ICountryService, CountryService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<ICityService, CityService>();
        services.AddScoped<IServiceTypeService, ServiceTypeService>();
        services.AddScoped<INumberTypeService, NumberTypeService>();
        services.AddScoped<IAccommodationTypeService, AccommodationTypeService>();
        services.AddScoped<IMealTypeService, MealTypeService>();
        services.AddScoped<IPartnerService, PartnerService>();
        services.AddScoped<ITourOperatorService, TourOperatorService>();
        services.AddScoped<ICitizenshipService, CitizenshipService>();
        services.AddScoped<IPartnerMarkService, PartnerMarkService>();
        services.AddScoped<IPartnerTypeService, PartnerTypeService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<IAuditLogger, AuditLogger>();
        services.AddScoped<IAuditQueryService, AuditQueryService>();
        services.AddScoped<ILeadStatusService, LeadStatusService>();
        services.AddScoped<ILeadSourceService, LeadSourceService>();
        services.AddScoped<ILeadRequestTypeService, LeadRequestTypeService>();
        services.AddScoped<ILabelService, LabelService>();
        services.AddScoped<IHotelService, HotelService>();
        services.AddScoped<ICurrencyService, CurrencyService>();
        services.AddScoped<ILeadSelectionService, LeadSelectionService>();
        services.AddScoped<IVisaTypeService, VisaTypeService>();
        services.AddScoped<ITariffService, TariffService>();
        services.AddScoped<ITariffPricingService, TariffPricingService>();
        return services;
    }
}