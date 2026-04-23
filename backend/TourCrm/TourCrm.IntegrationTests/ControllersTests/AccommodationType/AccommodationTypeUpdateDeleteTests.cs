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

public class AccommodationTypeUpdateDeleteTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Update_204_WithPermission()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IAccommodationTypeService>(f);
        svc.Setup(s => s.UpdateAsync(5, It.IsAny<UpdateAccommodationTypeDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/AccommodationType/5")
        {
            Content = JsonContent.Create(new UpdateAccommodationTypeDto { Name = "Хостел" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,EditDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_204_WithPermission()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IAccommodationTypeService>(f);
        svc.Setup(s => s.DeleteAsync(7, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/AccommodationType/7");
        req.Headers.Add("x-test-perms", "ViewDictionaries,DeleteDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_403_WithoutPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IAccommodationTypeService>(f);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/AccommodationType/5")
        {
            Content = JsonContent.Create(new UpdateAccommodationTypeDto { Name = "Хостел" })
        };
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}