using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.MealType;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.MealType;

public class MealTypeCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201_WithPermission()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IMealTypeService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateMealTypeDto>(), "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new MealTypeDto { Id = 10, Name = "AI" });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/MealType")
        {
            Content = JsonContent.Create(new CreateMealTypeDto { Name = "AI" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,CreateDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await resp.Content.ReadFromJsonAsync<MealTypeDto>();
        dto!.Id.Should().Be(10);
        dto.Name.Should().Be("AI");
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IMealTypeService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/MealType")
        {
            Content = JsonContent.Create(new CreateMealTypeDto { Name = "AI" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}