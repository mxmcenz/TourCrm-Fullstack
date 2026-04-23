using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Dictionaries.DealStatus;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.DealStatuses;

public class DealStatusesUpdateDeleteTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Update_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealStatusService>(f);
        svc.Setup(s => s.UpdateAsync(5, It.IsAny<UpdateDealStatusDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/DealStatuses/5")
        {
            Content = JsonContent.Create(new { name = "In progress" })
        };
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealStatusService>(f);
        svc.Setup(s => s.DeleteAsync(7, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var resp = await c.DeleteAsync("/api/DealStatuses/7");
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}