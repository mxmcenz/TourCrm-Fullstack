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

public class NumberTypeGetTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<INumberTypeService>(f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<NumberTypeDto> { new NumberTypeDto { Id = 1, Name = "Std" } });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/NumberType");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<NumberTypeDto>>();
        list!.Should().HaveCount(1);
        list[0].Id.Should().Be(1);
    }

    [Fact]
    public async Task GetById_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<INumberTypeService>(f);
        svc.Setup(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>()))
           .ReturnsAsync(new NumberTypeDto { Id = 5, Name = "Deluxe" });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/NumberType/5");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await resp.Content.ReadFromJsonAsync<NumberTypeDto>();
        dto!.Id.Should().Be(5);
    }

    [Fact]
    public async Task GetById_404()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<INumberTypeService>(f);
        svc.Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>()))
           .ReturnsAsync((NumberTypeDto?)null);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/NumberType/99");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
