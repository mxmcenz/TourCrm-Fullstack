using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Companies;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Entities;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Companies;

public class CompaniesRenameTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Rename_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ICompanyService>(f);
        svc.Setup(s => s.RenameAsync("1", "NewName", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Company { Id = 5, Name = "NewName", LegalEntityId = 3, LegalEntity = new(){ NameRu = "LE" }, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Companies/mine/name")
        {
            Content = JsonContent.Create(new CompanyUpsertDto{ Name = "NewName" })
        };
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await resp.Content.ReadFromJsonAsync<CompanyDto>();
        dto!.Name.Should().Be("NewName");
        dto.LegalEntityId.Should().Be(3);
        dto.LegalEntityName.Should().Be("LE");
    }

    [Fact]
    public async Task Rename_400_WhenNameEmpty()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ICompanyService>(f);
        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Companies/mine/name")
        {
            Content = JsonContent.Create(new CompanyUpsertDto{ Name = "" })
        };
        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}