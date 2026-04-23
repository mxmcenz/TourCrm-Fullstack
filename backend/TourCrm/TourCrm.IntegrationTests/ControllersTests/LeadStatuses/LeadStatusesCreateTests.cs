using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Dictionaries.LeadStatuses;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.LeadStatuses;

public class LeadStatusesCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadStatusService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateLeadStatusDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeadStatusDto(5, "В работе"));

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/LeadStatuses")
        {
            Content = JsonContent.Create(new { Name = "В работе" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,CreateDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await resp.Content.ReadFromJsonAsync<LeadStatusDto>();
        dto!.Id.Should().Be(5);
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ILeadStatusService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/LeadStatuses")
        {
            Content = JsonContent.Create(new { Name = "В работе" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}