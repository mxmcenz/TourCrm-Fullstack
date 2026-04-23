using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Citizenship;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Citizenship;

public class CitizenshipGetTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ICitizenshipService>(f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CitizenshipDto> { new() { Id = 1, Name = "KZ" } });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Citizenship");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<CitizenshipDto>>();
        list!.Count.Should().Be(1);
    }

    [Fact]
    public async Task Get_ById_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ICitizenshipService>(f);
        svc.Setup(s => s.GetByIdAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CitizenshipDto { Id = 2, Name = "RU" });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Citizenship/2");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_ById_404()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ICitizenshipService>(f);
        svc.Setup(s => s.GetByIdAsync(999, It.IsAny<CancellationToken>()))
            .ReturnsAsync((CitizenshipDto?)null);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Citizenship/999");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}