using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.DealStatuses;

public class DealStatusesAuthTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_Unauthorized()
    {
        var c = TestClient.Create(f);
        var resp = await c.GetAsync("/api/DealStatuses");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}