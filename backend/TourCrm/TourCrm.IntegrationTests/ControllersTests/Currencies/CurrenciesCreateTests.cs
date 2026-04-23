using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Dictionaries.Currencies;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Currencies;

public class CurrenciesCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ICurrencyService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateCurrencyDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CurrencyDto(10, "KZT"));

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Currencies")
        {
            Content = JsonContent.Create(new { name = "Kazakhstani tenge", code = "KZT" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,CreateDictionaries");
        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ICurrencyService>(f);
        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Currencies")
        {
            Content = JsonContent.Create(new { })
        };
        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}