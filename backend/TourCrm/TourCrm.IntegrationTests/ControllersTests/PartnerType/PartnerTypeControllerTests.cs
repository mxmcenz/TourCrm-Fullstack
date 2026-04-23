using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.PartnerType;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.PartnerType;

public class PartnerTypeControllerTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200_Empty()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IPartnerTypeService>(f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<PartnerTypeDto>());

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/PartnerType");
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await resp.Content.ReadFromJsonAsync<List<object>>();
        list!.Should().BeEmpty();
    }

    [Fact]
    public async Task Get_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IPartnerTypeService>(f);
        svc.Setup(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>()))
           .ReturnsAsync(new PartnerTypeDto{Id = 5, Name = "TO"});

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/PartnerType/5");
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_404()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IPartnerTypeService>(f);
        svc.Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>()))
           .ReturnsAsync((PartnerTypeDto?)null);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/PartnerType/99");
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IPartnerTypeService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreatePartnerTypeDto>(), "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new PartnerTypeDto{Id = 10, Name = "NewType"});

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/PartnerType")
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
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IPartnerTypeService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/PartnerType")
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
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IPartnerTypeService>(f);
        svc.Setup(s => s.UpdateAsync(7, It.IsAny<UpdatePartnerTypeDto>(), It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/PartnerType/7")
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
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IPartnerTypeService>(f);
        svc.Setup(s => s.DeleteAsync(3, It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/PartnerType/3");
        req.Headers.Add("x-test-perms", "ViewDictionaries,DeleteDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
