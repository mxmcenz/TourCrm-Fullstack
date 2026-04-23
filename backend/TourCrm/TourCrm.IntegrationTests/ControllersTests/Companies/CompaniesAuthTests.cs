using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Companies;

public class CompaniesAuthTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task AnyEndpoint_Unauthorized_WithoutAuth()
    {
        var c = TestClient.Create(f);
        var resp = await c.GetAsync("/api/Companies/mine");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}