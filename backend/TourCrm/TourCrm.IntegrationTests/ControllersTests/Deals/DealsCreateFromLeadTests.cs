using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Deals;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Deals;

public class DealsCreateFromLeadTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task CreateFromLead_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealService>(f);
        svc.Setup(s => s.CreateFromLeadAsync(11, 22, 33, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new DealDto { Id = 99, DealNumber = "D-99", StatusId = 1, ManagerId = 22, CompanyId = 123, TourName = "L" });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Deals/from-lead/11?managerId=22&touristId=33");
        req.Headers.Add("x-test-perms", "ViewDeals,CreateDeals");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task CreateFromLead_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IDealService>(f);
        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Deals/from-lead/11?managerId=22&touristId=33");
        req.Headers.Add("x-test-perms", "ViewDeals");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}