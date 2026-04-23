using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Clients;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Enums;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Clients;

public class ClientsUpdateDeleteRestoreTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Update_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IClientService>(f);
        svc.Setup(s => s.UpdateAsync(5, 777, 1, It.IsAny<UpdateClientDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Clients/5")
        {
            Content = JsonContent.Create(new UpdateClientDto { FirstName = "A", LastName = "B", ClientType = ClientType.PrivatePerson })
        };
        req.Headers.Add("x-test-perms", "ViewClients,EditClients");
        req.Headers.Add("x-test-company", "777");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IClientService>(f);
        svc.Setup(s => s.SoftDeleteAsync(9, 777, 1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/Clients/9");
        req.Headers.Add("x-test-perms", "ViewClients,DeleteClients");
        req.Headers.Add("x-test-company", "777");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Restore_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IClientService>(f);
        svc.Setup(s => s.RestoreAsync(9, 777, 1, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Clients/9/restore");
        req.Headers.Add("x-test-perms", "ViewClients,EditClients");
        req.Headers.Add("x-test-company", "777");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IClientService>(f);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Clients/5")
        {
            Content = JsonContent.Create(new UpdateClientDto { FirstName = "A", LastName = "B", ClientType = ClientType.PrivatePerson })
        };
        req.Headers.Add("x-test-company", "777");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}