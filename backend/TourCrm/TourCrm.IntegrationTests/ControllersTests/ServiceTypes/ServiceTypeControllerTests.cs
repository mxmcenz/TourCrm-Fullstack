using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.ServiceType;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.ServiceTypes;

public class ServiceTypeControllerTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200_Empty()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IServiceTypeService>(f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<ServiceTypeDto>());

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/ServiceType");
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await resp.Content.ReadFromJsonAsync<List<object>>();
        list!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_200_WithItems()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IServiceTypeService>(f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync([
                new ServiceTypeDto() { Id = 1, Name = "Flight" },
                new ServiceTypeDto() { Id = 2, Name = "Hotel" }
            ]);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/ServiceType");
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await resp.Content.ReadFromJsonAsync<List<object>>();
        list!.Should().HaveCount(2);
    }

    [Fact]
    public async Task Get_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IServiceTypeService>(f);
        svc.Setup(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>()))
           .ReturnsAsync(new ServiceTypeDto { Id = 5, Name = "Transfer" });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/ServiceType/5");
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Get_404()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IServiceTypeService>(f);
        svc.Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>()))
           .ReturnsAsync((ServiceTypeDto?)null);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/ServiceType/99");
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IServiceTypeService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateServiceTypeDto>(), "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new ServiceTypeDto { Id = 42, Name = "Excursion" });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/ServiceType")
        {
            Content = JsonContent.Create(new CreateServiceTypeDto { Name = "Excursion" })
        };
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "CreateDictionaries,ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Created);

        using var doc = await resp.Content.ReadFromJsonAsync<JsonDocument>();
        doc!.RootElement.GetProperty("id").GetInt32().Should().Be(42);
    }

    [Fact]
    public async Task Update_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IServiceTypeService>(f);
        svc.Setup(s => s.UpdateAsync(7, It.IsAny<UpdateServiceTypeDto>(), It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/ServiceType/7")
        {
            Content = JsonContent.Create(new UpdateServiceTypeDto { Name = "Tour" })
        };
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "EditDictionaries,ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IServiceTypeService>(f);
        svc.Setup(s => s.DeleteAsync(4, It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/ServiceType/4");
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "DeleteDictionaries,ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
