using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Citizenship;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Citizenship;

public class CitizenshipUpdateDeleteTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Update_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ICitizenshipService>(f);
        svc.Setup(s => s.UpdateAsync(5, It.IsAny<UpdateCitizenshipDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Citizenship/5")
        {
            Content = JsonContent.Create(new UpdateCitizenshipDto { Name = "UA" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,EditDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ICitizenshipService>(f);
        svc.Setup(s => s.DeleteAsync(7, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/Citizenship/7");
        req.Headers.Add("x-test-perms", "ViewDictionaries,DeleteDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ICitizenshipService>(f);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Citizenship/5")
        {
            Content = JsonContent.Create(new UpdateCitizenshipDto { Name = "UA" })
        };
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}