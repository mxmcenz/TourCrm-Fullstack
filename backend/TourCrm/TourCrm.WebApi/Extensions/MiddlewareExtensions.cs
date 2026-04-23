using TourCrm.WebApi.Middlewares;

namespace TourCrm.WebApi.Extensions;

public static class MiddlewareExtensions
{
    public static IApplicationBuilder UseLoggingContext(this IApplicationBuilder app)
    {
        return app.UseMiddleware<LoggingContextMiddleware>();
    }
}