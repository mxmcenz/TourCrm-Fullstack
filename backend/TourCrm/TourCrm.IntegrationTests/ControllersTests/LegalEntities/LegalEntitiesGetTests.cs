using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.LegalEntity;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.LegalEntities;

public class LegalEntitiesGetTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetMine_200_NoQuery()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILegalEntityService>(f);
        svc.Setup(s => s.GetMineAsync("1", null, It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<LegalEntityListItemDto> { new() { Id = 1, Name = "LE A" } });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/LegalEntities");
        req.Headers.Add("x-test-perms", "ViewLegalEntities");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<LegalEntityListItemDto>>();
        list!.Should().HaveCount(1);
        list[0].Id.Should().Be(1);
    }

    [Fact]
    public async Task GetMine_200_WithQueryParam()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILegalEntityService>(f);
        svc.Setup(s => s.GetMineAsync("1", "rom", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<LegalEntityListItemDto> { new() { Id = 2, Name = "Ромашка" } });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/LegalEntities?query=rom");
        req.Headers.Add("x-test-perms", "ViewLegalEntities");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<LegalEntityListItemDto>>();
        list!.Should().HaveCount(1);
        list[0].Id.Should().Be(2);
    }

    [Fact]
    public async Task Get_200_Found()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILegalEntityService>(f);
        svc.Setup(s => s.GetAsync(15, "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new LegalEntityDto { Id = 15, Name = "LE" });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/LegalEntities/15");
        req.Headers.Add("x-test-perms", "ViewLegalEntities");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await resp.Content.ReadFromJsonAsync<LegalEntityDto>();
        dto!.Id.Should().Be(15);
    }

    [Fact]
    public async Task Get_404_NotFound()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILegalEntityService>(f);
        svc.Setup(s => s.GetAsync(99, "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync((LegalEntityDto?)null);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/LegalEntities/99");
        req.Headers.Add("x-test-perms", "ViewLegalEntities");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
