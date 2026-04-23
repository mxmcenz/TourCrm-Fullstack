using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.AccommodationType;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.AccommodationType;

public class AccommodationTypeGetTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _f;
    public AccommodationTypeGetTests(WebApplicationFactory<Program> f) => _f = f;

    [Fact]
    public async Task GetAll_Returns200_WithPermission()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IAccommodationTypeService>(_f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<AccommodationTypeDto> { new() { Id = 1, Name = "Отель" } });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/AccommodationType");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<AccommodationTypeDto>>();
        list!.Count.Should().Be(1);
    }

    [Fact]
    public async Task Get_ById_200_WhenFound()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IAccommodationTypeService>(_f);
        svc.Setup(s => s.GetByIdAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccommodationTypeDto { Id = 1, Name = "Отель" });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/AccommodationType/1");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_ById_404_WhenNull()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IAccommodationTypeService>(_f);
        svc.Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((AccommodationTypeDto?)null);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/AccommodationType/999");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}