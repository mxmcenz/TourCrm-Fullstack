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

public class PartnerGetTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200_Empty()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IPartnerService>(f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<PartnerDto>()); // <-- вместо Array.Empty<PartnerDto>()

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Partner");
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await resp.Content.ReadFromJsonAsync<List<object>>();
        list!.Should().BeEmpty();
    }

    [Fact]
    public async Task Get_404()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IPartnerService>(f);
        svc.Setup(s => s.GetByIdAsync(123, It.IsAny<CancellationToken>()))
            .ReturnsAsync((PartnerDto?)null);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Partner/123");
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}