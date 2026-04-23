using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.DealStatuses;

public class DealStatusesSeedDefaultsTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task SeedDefaults_200_ReturnsInsertedCount()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IDealStatusService>(f);
        svc.Setup(s => s.SeedDefaultsForCompanyAsync(321, It.IsAny<CancellationToken>()))
            .ReturnsAsync(3);

        var resp = await c.PostAsync("/api/DealStatuses/seed-defaults/321", content: null);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await resp.Content.ReadFromJsonAsync<Dictionary<string,int>>();
        json!.Should().ContainKey("inserted");
        json["inserted"].Should().Be(3);
    }
}