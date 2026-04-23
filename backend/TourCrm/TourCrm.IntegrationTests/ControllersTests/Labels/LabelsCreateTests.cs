using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Dictionaries.Labels;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Labels;

public class LabelsCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILabelService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateLabelDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((LabelDto)null!);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Labels")
        {
            Content = JsonContent.Create(new { name = "VIP" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,CreateDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ILabelService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Labels")
        {
            Content = JsonContent.Create(new { name = "VIP" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}