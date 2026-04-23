using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Leads;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Leads;

public class LeadCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateLeadDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeadDto { Id = 5 });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Lead")
        {
            Content = JsonContent.Create(new
            {
                customerFirstName = "John",
                customerLastName = "Doe",
                phone = "+77001112233"
            })
        };
        req.Headers.Add("x-test-perms", "ViewLeads,CreateLeads");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await resp.Content.ReadFromJsonAsync<LeadDto>();
        dto!.Id.Should().Be(5);
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ILeadService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Lead")
        {
            Content = JsonContent.Create(new { })
        };
        req.Headers.Add("x-test-perms", "ViewLeads");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Create_409_OnOfficeLimit()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateLeadDto>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Лимит офиса исчерпан"));

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Lead")
        {
            Content = JsonContent.Create(new
            {
                customerFirstName = "John",
                customerLastName = "Doe",
                phone = "+77001112233"
            })
        };
        req.Headers.Add("x-test-perms", "ViewLeads,CreateLeads");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Conflict);
        var body = await resp.Content.ReadAsStringAsync();
        body.Should().Contain("Лимит офиса");
    }
}
