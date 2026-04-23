using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Dictionaries.Hotels;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Hotels;

public class HotelsUpdateDeleteTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Update_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IHotelService>(f);
        svc.Setup(s => s.UpdateAsync(5, It.IsAny<UpdateHotelDto>(), It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Hotels/5")
        {
            Content = JsonContent.Create(new { name = "New Name" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,EditDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IHotelService>(f);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Hotels/5")
        {
            Content = JsonContent.Create(new { name = "New Name" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IHotelService>(f);
        svc.Setup(s => s.DeleteAsync(7, It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/Hotels/7");
        req.Headers.Add("x-test-perms", "ViewDictionaries,DeleteDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IHotelService>(f);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/Hotels/7");
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
