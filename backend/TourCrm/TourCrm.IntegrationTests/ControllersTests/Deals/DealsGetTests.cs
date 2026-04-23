using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Deals;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Deals;

public class DealsGetTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealService>(f);
        svc.Setup(s => s.GetAllAsync(null, null, null, It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<DealDto> { new() { Id = 1, DealNumber = "D-1", StatusId = 1, ManagerId = 2, CompanyId = 123, TourName = "Tour" } });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Deals");
        req.Headers.Add("x-test-perms", "ViewDeals");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<DealDto>>();
        list!.Count.Should().Be(1);
    }

    [Fact]
    public async Task Get_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealService>(f);
        svc.Setup(s => s.GetAsync(10, It.IsAny<CancellationToken>()))
           .ReturnsAsync(new DealDto { Id = 10, DealNumber = "D-10", StatusId = 2, ManagerId = 5, CompanyId = 123, TourName = "X" });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Deals/10");
        req.Headers.Add("x-test-perms", "ViewDeals");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await resp.Content.ReadFromJsonAsync<DealDto>();
        dto!.Id.Should().Be(10);
    }

    [Fact]
    public async Task Get_404_WhenMissing()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealService>(f);
        svc.Setup(s => s.GetAsync(999, It.IsAny<CancellationToken>()))
           .ReturnsAsync((DealDto?)null);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Deals/999");
        req.Headers.Add("x-test-perms", "ViewDeals");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
