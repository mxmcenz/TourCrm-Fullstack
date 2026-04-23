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

public class CitizenshipCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ICitizenshipService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateCitizenshipDto>(), "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new CitizenshipDto { Id = 10, Name = "KG" });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Citizenship")
        {
            Content = JsonContent.Create(new CreateCitizenshipDto { Name = "KG" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,CreateDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ICitizenshipService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Citizenship")
        {
            Content = JsonContent.Create(new CreateCitizenshipDto { Name = "KG" })
        };
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}