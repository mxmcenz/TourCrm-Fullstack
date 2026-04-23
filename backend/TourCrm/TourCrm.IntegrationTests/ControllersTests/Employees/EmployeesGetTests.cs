using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Employees;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Employees;

public class EmployeesGetTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetAll_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        svc.Setup(s => s.GetAllAsync("1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<EmployeeDto> { new() { Id = 5, FirstName = "John", LastName = "Doe" } });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Employees");
        req.Headers.Add("x-test-perms", "ViewEmployees");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<EmployeeDto>>();
        list!.Should().HaveCount(1);
        list[0].Id.Should().Be(5);
    }

    [Fact]
    public async Task GetPaged_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        svc.Setup(s => s.GetPagedAsync(2, 5, null, null, "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new PagedResult<EmployeeDto>
           {
               Items = new List<EmployeeDto> { new() { Id = 2, FirstName = "A", LastName = "B" } },
               TotalCount = 10,
               Page = 2,
               PageSize = 5
           });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Employees/paged?page=2&pageSize=5");
        req.Headers.Add("x-test-perms", "ViewEmployees");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await resp.Content.ReadFromJsonAsync<PagedResult<EmployeeDto>>();
        dto!.TotalCount.Should().Be(10);
        dto.Items.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPaged_400_WhenInvalidPaging()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Employees/paged?page=0&pageSize=0");
        req.Headers.Add("x-test-perms", "ViewEmployees");
        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        svc.Setup(s => s.GetByIdAsync(15, "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new EmployeeDto { Id = 15, FirstName = "Jane", LastName = "D" });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Employees/15");
        req.Headers.Add("x-test-perms", "ViewEmployees");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var dto = await resp.Content.ReadFromJsonAsync<EmployeeDto>();
        dto!.Id.Should().Be(15);
    }

    [Fact]
    public async Task GetById_404_WithMessage()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        svc.Setup(s => s.GetByIdAsync(99, "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync((EmployeeDto?)null);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Employees/99");
        req.Headers.Add("x-test-perms", "ViewEmployees");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await resp.Content.ReadAsStringAsync();
        body.Should().Contain("99");
    }

    [Fact]
    public async Task GetByOffice_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        svc.Setup(s => s.GetByOfficeAsync(7, "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<EmployeeDto> { new() { Id = 1, FirstName = "O", LastName = "1" } });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Employees/office/7");
        req.Headers.Add("x-test-perms", "ViewEmployees");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetByOffice_404_WithMessage()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        svc.Setup(s => s.GetByOfficeAsync(123, "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new List<EmployeeDto>());

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Employees/office/123");
        req.Headers.Add("x-test-perms", "ViewEmployees");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
        var body = await resp.Content.ReadAsStringAsync();
        body.Should().Contain("123");
    }
}
