using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Citizenship;

public class CitizenshipAuthTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_Unauthorized()
    {
        var c = TestClient.Create(f);
        var resp = await c.GetAsync("/api/Citizenship");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_Forbidden_WithoutPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ICitizenshipService>(f);
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Citizenship");
        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}