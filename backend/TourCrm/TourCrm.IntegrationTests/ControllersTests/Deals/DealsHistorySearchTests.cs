using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Deals;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Specifications;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Deals;

public class DealsHistorySearchTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetHistory_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealService>(f);
        svc.Setup(s => s.GetHistoryAsync(5, It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<DealHistoryDto> { new() { Id = 1, Action = "created", CreatedAt = DateTime.UtcNow } });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Deals/5/history");
        req.Headers.Add("x-test-perms", "ViewDeals");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<DealHistoryDto>>();
        list!.Count.Should().Be(1);
    }

    [Fact]
    public async Task Search_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealService>(f);
        var page = new PagedResult<DealDto>(new List<DealDto> { new() { Id = 2, DealNumber = "D-2", StatusId = 1, ManagerId = 3, CompanyId = 123, TourName = "S" } }, 1);
        svc.Setup(s => s.SearchAsync(It.IsAny<DealSearchRequestDto>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(page);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Deals/search?page=1&pageSize=10");
        req.Headers.Add("x-test-perms", "ViewDeals");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var res = await resp.Content.ReadFromJsonAsync<PagedResult<DealDto>>();
        res!.Total.Should().Be(1);
        res.Items.Should().HaveCount(1);
    }
}
