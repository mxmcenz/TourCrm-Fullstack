using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Dictionaries.LeadStatuses;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.LeadStatuses;

public class LeadStatusesUpdateDeleteTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Update_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadStatusService>(f);
        svc.Setup(s => s.UpdateAsync(7, It.IsAny<UpdateLeadStatusDto>(), It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/LeadStatuses/7")
        {
            Content = JsonContent.Create(new { Name = "Обновлен" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,EditDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ILeadStatusService>(f);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/LeadStatuses/7")
        {
            Content = JsonContent.Create(new { Name = "Обновлен" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadStatusService>(f);
        svc.Setup(s => s.DeleteAsync(9, It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/LeadStatuses/9");
        req.Headers.Add("x-test-perms", "ViewDictionaries,DeleteDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ILeadStatusService>(f);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/LeadStatuses/9");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
