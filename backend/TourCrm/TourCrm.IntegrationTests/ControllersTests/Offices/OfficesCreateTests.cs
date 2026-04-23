using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Offices;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Offices;

public class OfficesCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IOfficeService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<OfficeUpsertDto>(), "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OfficeDto { Name = "New" });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Offices")
        {
            Content = JsonContent.Create(new OfficeUpsertDto { LegalEntityId = 7, Name = "New" })
        };
        req.Headers.Add("x-test-perms", "ViewOffices,CreateOffices");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await resp.Content.ReadFromJsonAsync<OfficeDto>();
        dto!.Name.Should().Be("New");
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IOfficeService>(f);
        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Offices")
        {
            Content = JsonContent.Create(new OfficeUpsertDto { LegalEntityId = 7, Name = "New" })
        };
        req.Headers.Add("x-test-perms", "ViewOffices");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}