using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Offices;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Offices;

public class OfficesUpdateDeleteTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Update_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IOfficeService>(f);
        svc.Setup(s => s.UpdateAsync(5, It.IsAny<OfficeUpsertDto>(), "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new OfficeDto { Name = "Updated" });

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Offices/5")
        {
            Content = JsonContent.Create(new OfficeUpsertDto { LegalEntityId = 7, Name = "Updated" })
        };
        req.Headers.Add("x-test-perms", "ViewOffices,EditOffices");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await resp.Content.ReadFromJsonAsync<OfficeDto>();
        dto!.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task Delete_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IOfficeService>(f);
        svc.Setup(s => s.SoftDeleteAsync(9, "1", It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/Offices/9");
        req.Headers.Add("x-test-perms", "ViewOffices,DeleteOffices");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}