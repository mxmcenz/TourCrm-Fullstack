using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ProgramTests;

public class ErrorMiddlewareTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _c = TestClient.Create(f);

    [Fact]
    public async Task ErrorHandling_Returns_ErrorJson()
    {
        var resp = await _c.GetAsync("/__throw");
        resp.StatusCode.Should().Be(HttpStatusCode.InternalServerError);

        var json = await resp.Content.ReadFromJsonAsync<Dictionary<string, string>>();
        json.Should().NotBeNull();
        json!.Should().ContainKey("error");
        json["error"].Should().NotBeNullOrEmpty();
    }
}