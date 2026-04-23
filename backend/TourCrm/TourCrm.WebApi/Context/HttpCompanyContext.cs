using TourCrm.Core.Abstractions;

namespace TourCrm.WebApi.Context;

public sealed class HttpCompanyContext : ICompanyContext
{
    public int CompanyId { get; }

    public HttpCompanyContext(IHttpContextAccessor accessor)
    {
        var http = accessor.HttpContext;

        var claim = http?.User?.FindFirst("CompanyId")
                    ?? http?.User?.FindFirst("cid")
                    ?? http?.User?.FindFirst("company_id");

        if (claim != null && int.TryParse(claim.Value, out var fromClaim) && fromClaim > 0)
        {
            CompanyId = fromClaim;
            return;
        }

        var header = http?.Request.Headers["X-Company-Id"].FirstOrDefault();
        if (int.TryParse(header, out var fromHeader) && fromHeader > 0)
        {
            CompanyId = fromHeader;
            return;
        }

        CompanyId = -1;
    }
}