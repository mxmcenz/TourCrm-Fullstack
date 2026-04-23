using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Deals;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Deals;

public class DealsUpdateStatusTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Update_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealService>(f);
        svc.Setup(s => s.UpdateAsync(5, It.IsAny<UpdateDealDto>(), It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Deals/5")
        {
            Content = JsonContent.Create(new { note = "x" })
        };
        req.Headers.Add("x-test-perms", "ViewDeals,EditDeals");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task ChangeStatus_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealService>(f);
        svc.Setup(s => s.ChangeStatusAsync(5, 3, It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Deals/5/status/3");
        req.Headers.Add("x-test-perms", "ViewDeals,EditDeals");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IDealService>(f);
        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Deals/5")
        {
            Content = JsonContent.Create(new { })
        };
        req.Headers.Add("x-test-perms", "ViewDeals");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
