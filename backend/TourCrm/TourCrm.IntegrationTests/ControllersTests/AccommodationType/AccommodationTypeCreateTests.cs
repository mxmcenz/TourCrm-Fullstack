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

public class AccommodationTypeCreateTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201_WithPermission()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IAccommodationTypeService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateAccommodationTypeDto>(), "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AccommodationTypeDto { Id = 10, Name = "Апартаменты" });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/AccommodationType")
        {
            Content = JsonContent.Create(new CreateAccommodationTypeDto { Name = "Апартаменты" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,CreateDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_403_WithoutPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IAccommodationTypeService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/AccommodationType")
        {
            Content = JsonContent.Create(new CreateAccommodationTypeDto { Name = "Апартаменты" })
        };
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}