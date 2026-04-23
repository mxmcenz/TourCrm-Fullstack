using System.Net;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Dictionaries.Labels;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Labels;

public class LabelsGetTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILabelService>(f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LabelDto>());

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Labels");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
        doc.RootElement.ValueKind.Should().Be(JsonValueKind.Array);
    }

    [Fact]
    public async Task GetAll_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ILabelService>(f);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Labels");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}