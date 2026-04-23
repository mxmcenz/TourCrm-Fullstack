using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.LegalEntity;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.LegalEntities;

public class LegalEntitiesCreateUpdateDeleteTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201_WithPermission()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILegalEntityService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<LegalEntityUpsertDto>(), "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new LegalEntityDto { Id = 7, Name = "New LE" });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/LegalEntities")
        {
            Content = JsonContent.Create(new LegalEntityUpsertDto { NameRu = "Новая", CompanyId = 1 })
        };
        req.Headers.Add("x-test-perms", "ViewLegalEntities,CreateLegalEntities");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await resp.Content.ReadFromJsonAsync<LegalEntityDto>();
        dto!.Id.Should().Be(7);
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ILegalEntityService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/LegalEntities")
        {
            Content = JsonContent.Create(new LegalEntityUpsertDto { NameRu = "N" })
        };
        req.Headers.Add("x-test-perms", "ViewLegalEntities");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_200_WithPermission()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILegalEntityService>(f);
        svc.Setup(s => s.UpdateAsync(5, It.IsAny<LegalEntityUpsertDto>(), "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new LegalEntityDto { Id = 5, Name = "Updated" });

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/LegalEntities/5")
        {
            Content = JsonContent.Create(new LegalEntityUpsertDto { NameRu = "Обновлённая" })
        };
        req.Headers.Add("x-test-perms", "ViewLegalEntities,EditLegalEntities");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await resp.Content.ReadFromJsonAsync<LegalEntityDto>();
        dto!.Id.Should().Be(5);
    }

    [Fact]
    public async Task Update_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ILegalEntityService>(f);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/LegalEntities/5")
        {
            Content = JsonContent.Create(new LegalEntityUpsertDto { NameRu = "X" })
        };
        req.Headers.Add("x-test-perms", "ViewLegalEntities");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Delete_204_WithPermission()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILegalEntityService>(f);
        svc.Setup(s => s.SoftDeleteAsync(9, "1", It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/LegalEntities/9");
        req.Headers.Add("x-test-perms", "ViewLegalEntities,DeleteLegalEntities");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ILegalEntityService>(f);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/LegalEntities/9");
        req.Headers.Add("x-test-perms", "ViewLegalEntities");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
