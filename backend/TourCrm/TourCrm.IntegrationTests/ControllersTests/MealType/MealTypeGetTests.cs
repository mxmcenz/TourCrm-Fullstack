using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs.MealType;
using TourCrm.Application.Interfaces;
using TourCrm.Application.Interfaces.Dictionaries;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.MealType;

public class MealTypeGetTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IMealTypeService>(f);
        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<MealTypeDto> { new MealTypeDto { Id = 1, Name = "BB" } });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/MealType");
        req.Headers.Add("x-test-perms", "ViewDictionaries");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<MealTypeDto>>();
        list!.Should().HaveCount(1);
        list[0].Should().BeEquivalentTo(new MealTypeDto { Id = 1, Name = "BB" });
    }

    [Fact]
    public async Task GetById_200_And_404()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IMealTypeService>(f);
        svc.Setup(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>()))
           .ReturnsAsync(new MealTypeDto { Id = 5, Name = "HB" });
        svc.Setup(s => s.GetByIdAsync(99, It.IsAny<CancellationToken>()))
           .ReturnsAsync((MealTypeDto?)null);

        var okReq = new HttpRequestMessage(HttpMethod.Get, "/api/MealType/5");
        okReq.Headers.Add("x-test-perms", "ViewDictionaries");
        var ok = await c.SendAsync(okReq);
        ok.StatusCode.Should().Be(HttpStatusCode.OK);

        var nfReq = new HttpRequestMessage(HttpMethod.Get, "/api/MealType/99");
        nfReq.Headers.Add("x-test-perms", "ViewDictionaries");
        var nf = await c.SendAsync(nfReq);
        nf.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
