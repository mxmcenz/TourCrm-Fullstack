using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.PartnerMark;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.PartnerMark;

public class PartnerMarkControllerTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IPartnerMarkService>(f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<PartnerMarkDto>());

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/PartnerMark");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<object>>();
        list!.Should().BeEmpty();
    }

    [Fact]
    public async Task Get_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IPartnerMarkService>(f);
        svc.Setup(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PartnerMarkDto { Id = 5, Name = "VIP" });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/PartnerMark/5");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_404()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IPartnerMarkService>(f);
        svc.Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>()))
           .ReturnsAsync((PartnerMarkDto?)null);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/PartnerMark/99");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IPartnerMarkService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreatePartnerMarkDto>(), "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new PartnerMarkDto{Id = 10, Name = "Новый"});

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/PartnerMark")
        {
            Content = JsonContent.Create(new { })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,CreateDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IPartnerMarkService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/PartnerMark")
        {
            Content = JsonContent.Create(new { })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }

    [Fact]
    public async Task Update_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IPartnerMarkService>(f);
        svc.Setup(s => s.UpdateAsync(7, It.IsAny<UpdatePartnerMarkDto>(), It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/PartnerMark/7")
        {
            Content = JsonContent.Create(new { })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,EditDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IPartnerMarkService>(f);
        svc.Setup(s => s.DeleteAsync(3, It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/PartnerMark/3");
        req.Headers.Add("x-test-perms", "ViewDictionaries,DeleteDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
