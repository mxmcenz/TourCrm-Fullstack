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

public class CompaniesCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ICompanyService>(f);
        svc.Setup(s => s.CreateAsync("1", "Acme", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Company { Id = 7, Name = "Acme", CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Companies")
        {
            Content = JsonContent.Create(new CompanyUpsertDto{ Name = "Acme" })
        };
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await resp.Content.ReadFromJsonAsync<CompanyDto>();
        dto!.Id.Should().Be(7);
        dto.Name.Should().Be("Acme");
    }

    [Fact]
    public async Task Create_400_WhenNameEmpty()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ICompanyService>(f);
        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Companies")
        {
            Content = JsonContent.Create(new CompanyUpsertDto{ Name = "  " })
        };
        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}