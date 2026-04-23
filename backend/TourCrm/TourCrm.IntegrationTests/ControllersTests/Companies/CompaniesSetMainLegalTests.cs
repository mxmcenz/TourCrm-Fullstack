using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Companies;

public class CompaniesSetMainLegalTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task SetMainLegal_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ICompanyService>(f);
        svc.Setup(s => s.SetMainLegalAsync("1", 12, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var resp = await c.PutAsync("/api/Companies/mine/main-legal/12", content: null);
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}