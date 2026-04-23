using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs.Tariffs;
using TourCrm.Application.Interfaces;
using TourCrm.Core.Enums;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Tariffs;

public class TariffsControllerTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    private static (HttpClient c, Mock<ITariffService> tariff, Mock<ITariffPricingService> pricing)
        MakeClient(WebApplicationFactory<Program> f)
        => TestClient.CreateWithHeaderAuthAndMocks<ITariffService, ITariffPricingService>(f);

    [Fact]
    public async Task GetAll_200_Empty()
    {
        var (c, tariff, _) = MakeClient(f);
        tariff.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<List<TariffDto>>.Ok(new List<TariffDto>()));

        var resp = await c.GetAsync("/api/Tariffs");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadFromJsonAsync<ServiceResult<List<TariffDto>>>();
        body!.Success.Should().BeTrue();
        body.Data.Should().NotBeNull().And.BeEmpty();
    }

    [Fact]
    public async Task Get_200_WithItem()
    {
        var (c, tariff, _) = MakeClient(f);
        var dto = new TariffDto(1, "Start", 1, 10, 10m, null, null, Array.Empty<TariffPermissionDto>());
        tariff.Setup(s => s.GetAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<TariffDto?>.Ok(dto));

        var resp = await c.GetAsync("/api/Tariffs/1");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadFromJsonAsync<ServiceResult<TariffDto?>>();
        body!.Success.Should().BeTrue();
        body.Data!.Name.Should().Be("Start");
    }

    [Fact]
    public async Task Suggest_200()
    {
        var (c, tariff, _) = MakeClient(f);
        var dto = new TariffDto(2, "Pro", 11, 50, 25m, 120m, 220m, Array.Empty<TariffPermissionDto>());
        tariff.Setup(s => s.SuggestForEmployeesAsync(15, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<TariffDto?>.Ok(dto));

        var resp = await c.GetAsync("/api/Tariffs/suggest?employees=15");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadFromJsonAsync<ServiceResult<TariffDto?>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(2);
    }

    [Fact]
    public async Task GetPrice_200()
    {
        var (c, _, pricing) = MakeClient(f);
        pricing.Setup(p => p.GetPriceAsync(3, BillingPeriod.Year, It.IsAny<CancellationToken>()))
            .ReturnsAsync(199m);

        var resp = await c.GetAsync("/api/Tariffs/3/price?period=Year");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var price = await resp.Content.ReadFromJsonAsync<decimal?>();
        price.Should().Be(199m);
    }

    [Fact]
    public async Task GetPricePerMonth_200()
    {
        var (c, _, pricing) = MakeClient(f);
        pricing.Setup(p => p.GetMonthlyEquivalentAsync(3, BillingPeriod.HalfYear, It.IsAny<CancellationToken>()))
            .ReturnsAsync(20m);

        var resp = await c.GetAsync("/api/Tariffs/3/price-per-month?period=HalfYear");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var price = await resp.Content.ReadFromJsonAsync<decimal?>();
        price.Should().Be(20m);
    }

    [Fact]
    public async Task Create_200_SuperAdmin()
    {
        var (c, tariff, _) = MakeClient(f);
        var created = new TariffDto(10, "Biz", 1, 100, 50m, 280m, 500m, Array.Empty<TariffPermissionDto>());
        tariff.Setup(s => s.CreateAsync(It.IsAny<CreateTariffDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<TariffDto>.Ok(created));

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Tariffs")
        {
            Content = JsonContent.Create(new CreateTariffDto(
                "Biz", 1, 100, 50m, 280m, 500m, Array.Empty<TariffPermissionDto>()))
        };
        req.Headers.Add("x-test-role", "SuperAdmin");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var body = await resp.Content.ReadFromJsonAsync<ServiceResult<TariffDto>>();
        body!.Success.Should().BeTrue();
        body.Data!.Id.Should().Be(10);
    }

    [Fact]
    public async Task Update_200_SuperAdmin()
    {
        var (c, tariff, _) = MakeClient(f);
        var updated = new TariffDto(5, "Plus", 2, 30, 20m, 110m, 200m, Array.Empty<TariffPermissionDto>());
        tariff.Setup(s => s.UpdateAsync(5, It.IsAny<UpdateTariffDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<TariffDto>.Ok(updated));

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Tariffs/5")
        {
            Content = JsonContent.Create(new UpdateTariffDto(
                "Plus", 2, 30, 20m, 110m, 200m, Array.Empty<TariffPermissionDto>()))
        };
        req.Headers.Add("x-test-role", "SuperAdmin");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_200_SuperAdmin()
    {
        var (c, tariff, _) = MakeClient(f);
        tariff.Setup(s => s.DeleteAsync(7, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<object>.Ok("deleted"));

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/Tariffs/7");
        req.Headers.Add("x-test-role", "SuperAdmin");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Assign_200_SuperAdmin()
    {
        var (c, tariff, _) = MakeClient(f);
        tariff.Setup(s => s.AssignToCompanyAsync(25, 4, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<object>.Ok("assigned"));

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Tariffs/4/assign?companyId=25");
        req.Headers.Add("x-test-role", "SuperAdmin");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Change_200_SuperAdmin()
    {
        var (c, tariff, _) = MakeClient(f);
        tariff.Setup(s => s.ChangeCompanyTariffAsync(25, 6, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<object>.Ok("changed"));

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Tariffs/6/change?companyId=25");
        req.Headers.Add("x-test-role", "SuperAdmin");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Unassign_200_SuperAdmin()
    {
        var (c, tariff, _) = MakeClient(f);
        tariff.Setup(s => s.RemoveCompanyTariffAsync(25, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<object>.Ok("unassigned"));

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/Tariffs/companies/25/assign");
        req.Headers.Add("x-test-role", "SuperAdmin");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}