using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Dictionaries.DealStatus;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.DealStatuses;

public class DealStatusesGetTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200_EmptyList()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealStatusService>(f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<DealStatusDto>());

        var resp = await c.GetAsync("/api/DealStatuses");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await resp.Content.ReadFromJsonAsync<List<DealStatusDto>>();
        list!.Count.Should().Be(0);
    }

    [Fact]
    public async Task GetById_404_WhenMissing()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealStatusService>(f);
        svc.Setup(s => s.GetByIdAsync(123, It.IsAny<CancellationToken>()))
            .ReturnsAsync((DealStatusDto?)null);

        var resp = await c.GetAsync("/api/DealStatuses/123");
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}