using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Clients;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Clients;

public class ClientsGetByIdTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetById_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IClientService>(f);
        svc.Setup(s => s.GetAsync(10, 321, false, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClientDetailsDto { Id = 10, CompanyId = 321, FirstName = "A", LastName = "B" });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Clients/10?includeDeleted=false");
        req.Headers.Add("x-test-perms", "ViewClients");
        req.Headers.Add("x-test-company", "321");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetById_404()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IClientService>(f);
        svc.Setup(s => s.GetAsync(999, 321, true, It.IsAny<CancellationToken>()))
            .ReturnsAsync((ClientDetailsDto?)null);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Clients/999?includeDeleted=true");
        req.Headers.Add("x-test-perms", "ViewClients");
        req.Headers.Add("x-test-company", "321");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}