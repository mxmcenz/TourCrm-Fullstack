using System.Diagnostics;
using System.Security.Claims;
using Serilog.Context;
using TourCrm.Core.Abstractions;

namespace TourCrm.WebApi.Middlewares;

public class LoggingContextMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
        context.Response.Headers["X-Request-Id"] = traceId;

        var userId = context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var companyCtx = context.RequestServices.GetService<ICompanyContext>();
        var companyId = (companyCtx != null && companyCtx.CompanyId > 0)
            ? companyCtx.CompanyId.ToString()
            : "unknown";

        using (LogContext.PushProperty("TraceId", traceId))
        using (LogContext.PushProperty("UserId", userId ?? "anonymous"))
        using (LogContext.PushProperty("CompanyId", companyId))
        {
            await next(context);
        }
    }
}