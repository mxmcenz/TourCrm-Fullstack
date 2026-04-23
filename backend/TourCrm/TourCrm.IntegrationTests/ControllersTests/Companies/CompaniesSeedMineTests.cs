using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Entities;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Companies;

public class CompaniesSeedMineTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task SeedMine_200_CallsSeederWithCompanyId()
    {
        var (c, companySvc, seeder) = TestClient.CreateWithHeaderAuthAndMocks<ICompanyService, IReferenceDataSeeder>(f);

        companySvc.Setup(s => s.GetMineAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Company { Id = 77, Name = "Mine" });

        seeder.Setup(s => s.SeedAllAsync(77, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var resp = await c.PostAsync("/api/Companies/seed/mine", content: null);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await resp.Content.ReadAsStringAsync();
        body.Should().Contain("Defaults seeded");
    }
}