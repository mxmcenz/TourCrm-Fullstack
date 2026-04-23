using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ProgramTests;

public class CorsPreflightTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _c = TestClient.Create(f);

    [Fact]
    public async Task Preflight_Healthz_Allows_Configured_Origin()
    {
        var req = new HttpRequestMessage(HttpMethod.Options, "/healthz");
        req.Headers.TryAddWithoutValidation("Origin", "http://localhost");
        req.Headers.TryAddWithoutValidation("Access-Control-Request-Method", "GET");

        var resp = await _c.SendAsync(req);
        resp.Headers.TryGetValues("Access-Control-Allow-Origin", out var origins).Should().BeTrue();
        origins!.Should().Contain("http://localhost");
    }
}