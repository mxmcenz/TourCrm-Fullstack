using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Leads;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.LeadSelections;

public class LeadSelectionsGetTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetSingle_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadSelectionService>(f);
        svc.Setup(s => s.GetSingleByLeadAsync(15, It.IsAny<string>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(new LeadSelectionDto { Id = 3 });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/leads/15/selections/single");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await resp.Content.ReadFromJsonAsync<LeadSelectionDto>();
        dto!.Id.Should().Be(3);
    }

    [Fact]
    public async Task GetSingle_404()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadSelectionService>(f);
        svc.Setup(s => s.GetSingleByLeadAsync(99, It.IsAny<string>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync((LeadSelectionDto?)null);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/leads/99/selections/single");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetById_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadSelectionService>(f);
        svc.Setup(s => s.GetAsync(10, 7, It.IsAny<string>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(new LeadSelectionDto { Id = 7 });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/leads/10/selections/7");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await resp.Content.ReadFromJsonAsync<LeadSelectionDto>();
        dto!.Id.Should().Be(7);
    }

    [Fact]
    public async Task GetById_404()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadSelectionService>(f);
        svc.Setup(s => s.GetAsync(10, 999, It.IsAny<string>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync((LeadSelectionDto?)null);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/leads/10/selections/999");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}

public class LeadSelectionsCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadSelectionService>(f);
        svc.Setup(s => s.CreateAsync(15, It.IsAny<CreateLeadSelectionDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeadSelectionDto { Id = 42 });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/leads/15/selections")
        {
            Content = JsonContent.Create(new { Price = 0m })
        };
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await resp.Content.ReadFromJsonAsync<LeadSelectionDto>();
        dto!.Id.Should().Be(42);
    }
}

public class LeadSelectionsUpdateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Update_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadSelectionService>(f);
        svc.Setup(s => s.UpdateAsync(15, 7, It.IsAny<UpdateLeadSelectionDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(new LeadSelectionDto { Id = 7 });

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/leads/15/selections/7")
        {
            Content = JsonContent.Create(new { })
        };
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await resp.Content.ReadFromJsonAsync<LeadSelectionDto>();
        dto!.Id.Should().Be(7);
    }
}
