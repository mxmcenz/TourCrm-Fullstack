using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthMeUnauthorizedTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Me_NoAuth_Returns401()
    {
        var c = TourCrm.IntegrationTests._Shared.TestClient.Create(f);
        var resp = await c.GetAsync("/api/Auth/me");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}