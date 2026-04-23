using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Dictionaries.Currencies;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Currencies;

public class CurrenciesGetTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ICurrencyService>(f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<CurrencyDto>());

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Currencies");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<CurrencyDto>>();
        list!.Count.Should().Be(0);
    }
}