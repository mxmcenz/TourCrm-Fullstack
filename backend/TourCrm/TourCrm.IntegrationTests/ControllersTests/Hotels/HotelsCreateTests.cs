using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Dictionaries.Hotels;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Hotels;

public class HotelsCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IHotelService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateHotelDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((HotelDto)null!);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Hotels")
        {
            Content = JsonContent.Create(new { name = "Hotel A" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,CreateDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IHotelService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Hotels")
        {
            Content = JsonContent.Create(new { name = "Hotel A" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}