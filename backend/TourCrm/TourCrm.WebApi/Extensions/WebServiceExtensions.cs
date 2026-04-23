using FluentValidation;
using FluentValidation.AspNetCore;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Validations.Leads;
using TourCrm.Core.Abstractions;
using TourCrm.Infrastructure.Data;
using TourCrm.Infrastructure.Services;
using TourCrm.WebApi.Context;
using TourCrm.WebApi.Startup;

namespace TourCrm.WebApi.Extensions;

public static class WebServiceExtensions
{
    public static IServiceCollection AddWebServices(this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<CreateLeadDtoValidator>();
        services.AddValidatorsFromAssemblyContaining<CreateLeadSelectionDtoValidator>();

        services.AddHttpContextAccessor();
        services.AddScoped<ICompanyContext, HttpCompanyContext>();
        services.AddScoped<DbInitializer>();

        return services;
    }

    public static IServiceCollection AddPermissionServices(this IServiceCollection services)
    {
        services.AddSingleton<IPermissionProvider>(sp =>
        {
            var env = sp.GetRequiredService<IWebHostEnvironment>();
            var permissionsPath = Path.Combine(env.ContentRootPath, "permissions.json");
            return new JsonPermissionProvider(permissionsPath);
        });

        services.AddHostedService<PermissionsIntegrityHostedService>();

        return services;
    }
}