using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace TourCrm.IntegrationTests._Shared;

public sealed class TestHeaderAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> o, ILoggerFactory l, UrlEncoder e)
    : AuthenticationHandler<AuthenticationSchemeOptions>(o, l, e)
{
    public const string Scheme = "TestHeader";

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new List<Claim> { new(ClaimTypes.NameIdentifier, "1") };

        if (Request.Headers.TryGetValue("x-test-role", out var role) && !string.IsNullOrWhiteSpace(role))
            claims.Add(new Claim(ClaimTypes.Role, role!));

        if (Request.Headers.TryGetValue("x-test-perms", out var perms))
            foreach (var p in perms.ToString().Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                claims.Add(new Claim("permissions", p));
        
        if (Request.Headers.TryGetValue("x-test-company", out var company) && !string.IsNullOrWhiteSpace(company))
            claims.Add(new Claim("CompanyId", company!));

        var id = new ClaimsIdentity(claims, Scheme);
        return Task.FromResult(AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(id), Scheme)));
    }
}