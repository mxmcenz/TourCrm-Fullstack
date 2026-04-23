using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Deals;

public class DealsAuthTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_Unauthorized()
    {
        var c = TestClient.Create(f);
        var resp = await c.GetAsync("/api/Deals");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_Forbidden_WithoutPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IDealService>(f);
        var resp = await c.GetAsync("/api/Deals");
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}