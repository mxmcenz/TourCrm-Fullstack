namespace TourCrm.WebApi.Extensions;

public static class CorsExtensions
{
    public static IServiceCollection AddAppCors(this IServiceCollection services, IConfiguration configuration)
    {
        var origins = (configuration["FrontendOrigins"] ?? "http://localhost")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        services.AddCors(o => o.AddPolicy("AllowFrontend",
            p => p.WithOrigins(origins)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()));

        return services;
    }
}