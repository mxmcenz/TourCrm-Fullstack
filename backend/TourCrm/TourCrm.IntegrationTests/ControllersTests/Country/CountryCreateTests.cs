using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Dictionaries.Countries;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Country;

public class CountryCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ICountryService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateCountryDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CountryDto(10, "Kyrgyzstan"));

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Country")
        {
            Content = JsonContent.Create(new CreateCountryDto("Kyrgyzstan"))
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,CreateDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ICountryService>(f);
        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Country")
        {
            Content = JsonContent.Create(new CreateCountryDto("Kyrgyzstan"))
        };
        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}