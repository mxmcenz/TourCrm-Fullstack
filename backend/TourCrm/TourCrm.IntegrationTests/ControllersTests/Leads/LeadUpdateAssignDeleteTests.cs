using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Leads;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Leads;

public class LeadUpdateAssignDeleteTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Update_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadService>(f);
        svc.Setup(s => s.UpdateAsync(7, It.IsAny<UpdateLeadDto>(), It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Lead/7")
        {
            Content = JsonContent.Create(new { })
        };
        req.Headers.Add("x-test-perms", "ViewLeads,EditLeads");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Assign_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadService>(f);
        svc.Setup(s => s.AssignUserAsync(7, 123, It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Lead/7/assign")
        {
            Content = JsonContent.Create(new AssignUserToLeadDto { UserId = 123 })
        };
        req.Headers.Add("x-test-perms", "ViewLeads,EditLeads");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Assign_400_WhenBodyNull()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ILeadService>(f);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Lead/7/assign")
        {
            Content = null
        };
        req.Headers.Add("x-test-perms", "ViewLeads,EditLeads");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Assign_409_OnOfficeLimit()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadService>(f);
        svc.Setup(s => s.AssignUserAsync(7, 123, It.IsAny<CancellationToken>()))
           .ThrowsAsync(new InvalidOperationException("Лимит офиса достигли"));

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Lead/7/assign")
        {
            Content = JsonContent.Create(new AssignUserToLeadDto { UserId = 123 })
        };
        req.Headers.Add("x-test-perms", "ViewLeads,EditLeads");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Fact]
    public async Task Delete_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadService>(f);
        svc.Setup(s => s.DeleteAsync(8, It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/Lead/8");
        req.Headers.Add("x-test-perms", "ViewLeads,DeleteLeads");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
