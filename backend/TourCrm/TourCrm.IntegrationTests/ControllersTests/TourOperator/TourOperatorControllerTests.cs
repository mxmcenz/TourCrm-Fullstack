using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.TourOperator;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.TourOperator;

public class TourOperatorControllerTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200_Empty()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ITourOperatorService>(f);

        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TourOperatorDto>());

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/TourOperator");
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await resp.Content.ReadFromJsonAsync<List<object>>();
        list!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_200_WithItems()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ITourOperatorService>(f);

        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TourOperatorDto>
            {
                new() { Id = 1, Name = "TUI" },
                new() { Id = 2, Name = "Tez Tour" }
            });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/TourOperator");
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await resp.Content.ReadFromJsonAsync<List<TourOperatorDto>>();
        list!.Should().HaveCount(2);
        list[0].Name.Should().Be("TUI");
    }

    [Fact]
    public async Task Get_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ITourOperatorService>(f);

        svc.Setup(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new TourOperatorDto { Id = 5, Name = "Coral" });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/TourOperator/5");
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await resp.Content.ReadFromJsonAsync<TourOperatorDto>();
        dto!.Id.Should().Be(5);
        dto.Name.Should().Be("Coral");
    }

    [Fact]
    public async Task Get_404()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ITourOperatorService>(f);

        svc.Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TourOperatorDto?)null);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/TourOperator/99");
        req.Headers.Add("x-test-perms", "ViewDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ITourOperatorService>(f);

        var created = new TourOperatorDto { Id = 10, Name = "Pegas" };

        svc.Setup(s =>
                s.CreateAsync(It.IsAny<CreateTourOperatorDto>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(created);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/TourOperator")
        {
            Content = JsonContent.Create(new CreateTourOperatorDto { Name = "Pegas" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,CreateDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Created);

        var dto = await resp.Content.ReadFromJsonAsync<TourOperatorDto>();
        dto!.Id.Should().Be(10);
        dto.Name.Should().Be("Pegas");
    }

    [Fact]
    public async Task Update_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ITourOperatorService>(f);

        svc.Setup(s => s.UpdateAsync(7, It.IsAny<UpdateTourOperatorDto>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/TourOperator/7")
        {
            Content = JsonContent.Create(new UpdateTourOperatorDto { Name = "Updated" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,EditDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ITourOperatorService>(f);

        svc.Setup(s => s.DeleteAsync(4, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/TourOperator/4");
        req.Headers.Add("x-test-perms", "ViewDictionaries,DeleteDictionaries");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}