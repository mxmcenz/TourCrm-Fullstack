using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ProgramTests;

public class RootAndHealthTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _c = TestClient.Create(f);

    [Fact]
    public async Task Root_Returns_OK_Text()
    {
        var resp = await _c.GetAsync("/");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var text = await resp.Content.ReadAsStringAsync();
        text.Should().Be("OK");
    }

    [Fact]
    public async Task Healthz_Returns_Healthy_Json()
    {
        var resp = await _c.GetAsync("/healthz");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        json!["status"].Should().Be("healthy");
    }
}