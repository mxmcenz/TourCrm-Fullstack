using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Dictionaries.LeadSources;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.LeadSources;

public class LeadSourcesCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadSourceService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateLeadSourceDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeadSourceDto(5, "Website"));

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/LeadSources")
        {
            Content = JsonContent.Create(new { Name = "Website" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,CreateDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await resp.Content.ReadFromJsonAsync<LeadSourceDto>();
        dto!.Id.Should().Be(5);
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ILeadSourceService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/LeadSources")
        {
            Content = JsonContent.Create(new { Name = "Website" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}