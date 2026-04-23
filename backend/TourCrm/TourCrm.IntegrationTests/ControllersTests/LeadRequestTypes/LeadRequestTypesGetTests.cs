using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Dictionaries.LeadRequestTypes;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.LeadRequestTypes;

public class LeadRequestTypesGetTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadRequestTypeService>(f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<LeadRequestTypeDto> { new(1, "Звонок") });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/LeadRequestTypes");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<LeadRequestTypeDto>>();
        list!.Should().HaveCount(1);
        list[0].Id.Should().Be(1);
        list[0].Name.Should().Be("Звонок");
    }
}

public class LeadRequestTypesCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadRequestTypeService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<CreateLeadRequestTypeDto>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(new LeadRequestTypeDto(10, "Онлайн-заявка"));

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/LeadRequestTypes")
        {
            Content = JsonContent.Create(new { name = "Онлайн-заявка" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,CreateDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await resp.Content.ReadFromJsonAsync<LeadRequestTypeDto>();
        dto!.Id.Should().Be(10);
        dto.Name.Should().Be("Онлайн-заявка");
    }

    [Fact]
    public async Task Create_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<ILeadRequestTypeService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/LeadRequestTypes")
        {
            Content = JsonContent.Create(new { name = "X" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}

public class LeadRequestTypesUpdateDeleteTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Update_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadRequestTypeService>(f);
        svc.Setup(s => s.UpdateAsync(5, It.IsAny<UpdateLeadRequestTypeDto>(), It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/LeadRequestTypes/5")
        {
            Content = JsonContent.Create(new { name = "Переписка" })
        };
        req.Headers.Add("x-test-perms", "ViewDictionaries,EditDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<ILeadRequestTypeService>(f);
        svc.Setup(s => s.DeleteAsync(7, It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/LeadRequestTypes/7");
        req.Headers.Add("x-test-perms", "ViewDictionaries,DeleteDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
