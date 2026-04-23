using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Interfaces;
using TourCrm.Application.DTOs.Partner;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Partners;

public class PartnerCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IPartnerService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreatePartnerDto>(), "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((PartnerDto)null!);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Partner")
        {
            Content = JsonContent.Create(new { /* минимальный body */ })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,CreateDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IPartnerService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Partner")
        {
            Content = JsonContent.Create(new { })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}