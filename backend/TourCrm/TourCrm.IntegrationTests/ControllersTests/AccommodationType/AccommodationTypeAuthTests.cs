using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.AccommodationType;

public class AccommodationTypeAuthTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_Unauthorized_WithoutAuth()
    {
        var c = TestClient.Create(f);
        var resp = await c.GetAsync("/api/AccommodationType");
        resp.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAll_Forbidden_WithoutPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IAccommodationTypeService>(f);
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/AccommodationType");
        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}