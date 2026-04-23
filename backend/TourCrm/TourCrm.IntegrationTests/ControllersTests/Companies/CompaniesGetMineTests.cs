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

public class CompaniesGetMineTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetMine_200_ReturnsDto()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ICompanyService>(f);
        svc.Setup(s => s.GetMineAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Company { Id = 5, Name = "Acme", LegalEntityId = 11, LegalEntity = new LegalEntity{ NameRu = "ООО Ромашка"}, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Companies/mine");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await resp.Content.ReadFromJsonAsync<CompanyDto>();
        dto!.Id.Should().Be(5);
        dto.LegalEntityId.Should().Be(11);
        dto.LegalEntityName.Should().Be("ООО Ромашка");
    }

    [Fact]
    public async Task GetMine_200Or204_Null()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ICompanyService>(f);
        svc.Setup(s => s.GetMineAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync((Company?)null);

        var resp = await c.GetAsync("/api/Companies/mine");

        resp.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.NoContent);

        if (resp.StatusCode == HttpStatusCode.OK)
        {
            var json = await resp.Content.ReadAsStringAsync();
            json.Trim().Should().Be("null");
        }
        else
        {
            var body = await resp.Content.ReadAsStringAsync();
            body.Should().BeEmpty();
        }
    }
}