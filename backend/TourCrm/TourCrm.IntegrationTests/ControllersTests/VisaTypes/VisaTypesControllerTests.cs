using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.Dictionaries.VisaTypes;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.VisaTypes;

public class VisaTypesControllerTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200_Empty()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IVisaTypeService>(f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<VisaTypeDto>());

        var resp = await c.GetAsync("/api/VisaTypes");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await resp.Content.ReadFromJsonAsync<List<VisaTypeDto>>();
        list!.Should().BeEmpty();
    }

    [Fact]
    public async Task GetAll_200_WithItems()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IVisaTypeService>(f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<VisaTypeDto>
           {
               new(1, "Tourist"),
               new(2, "Business")
           });

        var resp = await c.GetAsync("/api/VisaTypes");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await resp.Content.ReadFromJsonAsync<List<VisaTypeDto>>();
        list!.Should().HaveCount(2);
        list[0].Name.Should().Be("Tourist");
    }

    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IVisaTypeService>(f);
        var created = new VisaTypeDto(10, "Transit");

        svc.Setup(s => s.CreateAsync(It.IsAny<CreateVisaTypeDto>(), It.IsAny<CancellationToken>()))
           .ReturnsAsync(created);

        var resp = await c.PostAsJsonAsync("/api/VisaTypes", new CreateVisaTypeDto("Transit"));
        resp.StatusCode.Should().Be(HttpStatusCode.Created);

        var dto = await resp.Content.ReadFromJsonAsync<VisaTypeDto>();
        dto!.Id.Should().Be(10);
    }

    [Fact]
    public async Task Update_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IVisaTypeService>(f);
        svc.Setup(s => s.UpdateAsync(5, It.IsAny<UpdateVisaTypeDto>(), It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var resp = await c.PutAsJsonAsync("/api/VisaTypes/5", new UpdateVisaTypeDto("Updated"));
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IVisaTypeService>(f);
        svc.Setup(s => s.DeleteAsync(7, It.IsAny<CancellationToken>()))
           .Returns(Task.CompletedTask);

        var resp = await c.DeleteAsync("/api/VisaTypes/7");
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }
}
