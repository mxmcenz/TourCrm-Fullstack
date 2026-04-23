using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Leads;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Leads;

public class LeadGetTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadService>(f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<LeadDto> { new() { Id = 1 } });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Lead");
        req.Headers.Add("x-test-perms", "ViewLeads");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<LeadDto>>();
        list!.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetById_200_And_404()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadService>(f);
        svc.Setup(s => s.GetByIdAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new LeadDto { Id = 10 });
        svc.Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((LeadDto?)null);

        var okReq = new HttpRequestMessage(HttpMethod.Get, "/api/Lead/10");
        okReq.Headers.Add("x-test-perms", "ViewLeads");
        var ok = await c.SendAsync(okReq);
        ok.StatusCode.Should().Be(HttpStatusCode.OK);

        var nfReq = new HttpRequestMessage(HttpMethod.Get, "/api/Lead/99");
        nfReq.Headers.Add("x-test-perms", "ViewLeads");
        var nf = await c.SendAsync(nfReq);
        nf.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}