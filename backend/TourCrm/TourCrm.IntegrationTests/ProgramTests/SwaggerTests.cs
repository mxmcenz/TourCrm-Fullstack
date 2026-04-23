using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ProgramTests;

public class SwaggerTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _c = TestClient.Create(f);

    [Fact]
    public async Task Swagger_V1_Json_Available()
    {
        var resp = await _c.GetAsync("/swagger/v1/swagger.json");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        resp.Content.Headers.ContentType!.MediaType.Should().Be("application/json");

        var body = await resp.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(body);

        doc.RootElement.GetProperty("openapi").GetString()!.StartsWith("3.").Should().BeTrue();
        doc.RootElement.GetProperty("info").GetProperty("title").GetString().Should().Be("TourCrm API");
        doc.RootElement.GetProperty("info").GetProperty("version").GetString().Should().Be("v1");
    }
}