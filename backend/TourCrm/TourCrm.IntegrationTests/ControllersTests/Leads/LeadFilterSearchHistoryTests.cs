using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Leads;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Leads;

public class LeadFilterSearchHistoryTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Filter_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadService>(f);
        svc.Setup(s => s.FilterByStatusAsync("new", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<LeadDto> { new() { Id = 1 } });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Lead/filter?status=new");
        req.Headers.Add("x-test-perms", "ViewLeads");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<LeadDto>>();
        list!.Should().HaveCount(1);
    }

    [Fact]
    public async Task Filter_400_WhenMissing()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ILeadService>(f);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Lead/filter");
        req.Headers.Add("x-test-perms", "ViewLeads");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Search_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadService>(f);
        svc.Setup(s => s.SearchAsync(It.IsAny<LeadFilterDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeadPageDto
            {
                Items = new List<LeadListItemDto> { new() { Id = 2 } },
                Total = 1
            });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Lead/search?page=1&pageSize=10");
        req.Headers.Add("x-test-perms", "ViewLeads");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var page = await resp.Content.ReadFromJsonAsync<LeadPageDto>();
        page!.Total.Should().Be(1);
        page.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task History_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadService>(f);
        svc.Setup(s => s.GetHistoryAsync(15, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LeadHistoryDto>
            {
                new() { Action = "created", CreatedAt = DateTime.UtcNow }
            });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Lead/15/history");
        req.Headers.Add("x-test-perms", "ViewLeads");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var items = await resp.Content.ReadFromJsonAsync<List<LeadHistoryDto>>();
        items!.Should().HaveCount(1);
    }
}
