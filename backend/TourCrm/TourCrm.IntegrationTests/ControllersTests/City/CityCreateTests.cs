using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.City;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.City;

public class CityCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ICityService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateCityDto>(), "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CityDto { Id = 10, Name = "Shymkent" });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/City")
        {
            Content = JsonContent.Create(new CreateCityDto { Name = "Shymkent" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,CreateDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ICityService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/City")
        {
            Content = JsonContent.Create(new CreateCityDto { Name = "Shymkent" })
        };
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}