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

public class ClientsCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201_PassesCompanyAndUser()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IClientService>(f);

        svc.Setup(s => s.CreateAsync(
                It.Is<int>(cid => cid == 777),
                It.Is<int?>(uid => uid == 1),
                It.IsAny<CreateClientDto>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ClientDetailsDto { Id = 100, CompanyId = 777, FirstName = "N", LastName = "M", ClientType = ClientType.PrivatePerson });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Clients")
        {
            Content = JsonContent.Create(new CreateClientDto { FirstName = "N", LastName = "M", ClientType = ClientType.PrivatePerson })
        };
        req.Headers.Add("x-test-perms", "ViewClients,CreateClients");
        req.Headers.Add("x-test-company", "777");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IClientService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Clients")
        {
            Content = JsonContent.Create(new CreateClientDto { FirstName = "N", LastName = "M", ClientType = ClientType.PrivatePerson })
        };
        req.Headers.Add("x-test-company", "777");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}