using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Deals;

public class DealsArchiveRestoreTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Archive_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealService>(f);
        svc.Setup(s => s.ArchiveAsync(8, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/Deals/8");
        req.Headers.Add("x-test-perms", "ViewDeals");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Restore_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealService>(f);
        svc.Setup(s => s.RestoreAsync(8, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Deals/8/restore");
        req.Headers.Add("x-test-perms", "ViewDeals");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}