using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Clients;

public class ClientsGetHistoryTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetHistory_200_AndTotalHeader()
    {
        var (c, _, audit) = TestClient.CreateWithHeaderAuthAndMocks<IClientService, IAuditQueryService>(f);

        audit.Setup(a => a.GetByEntityAsync(321, "Client", "15", 2, 50, It.IsAny<CancellationToken>()))
            .ReturnsAsync((
                new List<AuditLogDto>
                {
                    new()
                    {
                        Id = 1,
                        CompanyId = 321,
                        Entity = "Client",
                        EntityId = "15",
                        Action = "Update",
                        Data = JsonDocument.Parse("{}").RootElement,
                        AtUtc = DateTime.UtcNow
                    }
                },
                3));

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Clients/15/history?page=2&pageSize=50");
        req.Headers.Add("x-test-perms", "ViewClients");
        req.Headers.Add("x-test-company", "321");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        resp.Headers.TryGetValues("X-Total-Count", out var total).Should().BeTrue();
        total!.Should().ContainSingle().Which.Should().Be("3");

        var list = await resp.Content.ReadFromJsonAsync<List<AuditLogDto>>();
        list!.Count.Should().Be(1);
    }
}