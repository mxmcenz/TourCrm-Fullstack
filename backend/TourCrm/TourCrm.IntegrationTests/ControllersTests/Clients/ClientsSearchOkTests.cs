using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Clients;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Clients;

public class ClientsSearchOkTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Search_Returns200_AndTotalHeader()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IClientService>(f);
        svc.Setup(s => s.SearchAsync(
                It.Is<int>(cid => cid == 123),
                It.Is<string?>(q => q == "ann"),
                It.Is<int>(p => p == 2),
                It.Is<int>(ps => ps == 10),
                It.Is<bool>(inc => inc == true),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<ClientListItemDto> { new() { Id = 1, FirstName = "Ann", LastName = "Lee" } }, 42));

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Clients?q=ann&page=2&pageSize=10&includeDeleted=true");
        req.Headers.Add("x-test-perms", "ViewClients");
        req.Headers.Add("x-test-company", "123");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        resp.Headers.TryGetValues("X-Total-Count", out var total).Should().BeTrue();
        total!.Should().ContainSingle().Which.Should().Be("42");

        var list = await resp.Content.ReadFromJsonAsync<List<ClientListItemDto>>();
        list!.Count.Should().Be(1);
    }

    [Fact]
    public async Task SearchDeleted_Returns200_AndTotalHeader()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IClientService>(f);
        svc.Setup(s => s.SearchDeletedAsync(
                It.Is<int>(cid => cid == 555),
                It.Is<string?>(q => q == "x"),
                It.Is<int>(p => p == 3),
                It.Is<int>(ps => ps == 5),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<ClientListItemDto> { new() { Id = 2, FirstName = "X", LastName = "Y", IsDeleted = true } }, 7));

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Clients/deleted?q=x&page=3&pageSize=5");
        req.Headers.Add("x-test-perms", "ViewClients");
        req.Headers.Add("x-test-company", "555");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        resp.Headers.TryGetValues("X-Total-Count", out var total).Should().BeTrue();
        total!.Should().ContainSingle().Which.Should().Be("7");
    }
}