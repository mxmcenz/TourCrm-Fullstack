using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Offices;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Offices;

public class OfficesGetTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Get_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IOfficeService>(f);
        svc.Setup(s => s.GetAsync(5, "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new OfficeDto { Name = "Main" });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Offices/5");
        req.Headers.Add("x-test-perms", "ViewOffices");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await resp.Content.ReadFromJsonAsync<OfficeDto>();
        dto!.Name.Should().Be("Main");
    }

    [Fact]
    public async Task Get_404()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IOfficeService>(f);
        svc.Setup(s => s.GetAsync(99, "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync((OfficeDto?)null);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Offices/99");
        req.Headers.Add("x-test-perms", "ViewOffices");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetByLegal_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IOfficeService>(f);
        svc.Setup(s => s.GetByLegalAsync("1", 11, "abc", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<OfficeListItemDto> { new() { Id = 2, Name = "Branch" } });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Offices/by-legal/11?q=abc");
        req.Headers.Add("x-test-perms", "ViewOffices");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<OfficeListItemDto>>();
        list!.Should().HaveCount(1);
        list[0].Id.Should().Be(2);
    }
}
