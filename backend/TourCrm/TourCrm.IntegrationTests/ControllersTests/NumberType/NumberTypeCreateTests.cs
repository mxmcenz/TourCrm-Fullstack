using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.NumberType;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.NumberType;

public class NumberTypeCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<INumberTypeService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateNumberTypeDto>(), "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new NumberTypeDto { Id = 10, Name = "Suite" });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/NumberType")
        {
            Content = JsonContent.Create(new CreateNumberTypeDto { Name = "Suite" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,CreateDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await resp.Content.ReadFromJsonAsync<NumberTypeDto>();
        dto!.Id.Should().Be(10);
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<INumberTypeService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/NumberType")
        {
            Content = JsonContent.Create(new CreateNumberTypeDto { Name = "Suite" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}