using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Deals;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Deals;

public class DealsCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateDealDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DealDto { Id = 7, DealNumber = "D-7", StatusId = 1, ManagerId = 2, CompanyId = 123, TourName = "T" });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Deals")
        {
            Content = JsonContent.Create(new { managerId = 2, tourName = "T" })
        };
        req.Headers.Add("x-test-perms", "ViewDeals,CreateDeals");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IDealService>(f);
        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Deals")
        {
            Content = JsonContent.Create(new { managerId = 2, tourName = "T" })
        };
        req.Headers.Add("x-test-perms", "ViewDeals");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}