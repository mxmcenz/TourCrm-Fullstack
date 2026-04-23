using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Employees;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Employees;

public class EmployeesCreateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        svc.Setup(s => s.CreateAsync(It.IsAny<EmployeeCreateDto>(), "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(new EmployeeDto { Id = 7, FirstName = "John", LastName = "Doe" });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Employees")
        {
            Content = JsonContent.Create(new
            {
                officeId = 1,
                legalEntityId = 2,
                email = "john@doe.local",
                firstName = "John",
                lastName = "Doe",
                phone = "+111111",
                password = "P@ssw0rd!",
                leadLimit = 10,
                isDeleted = false,
                roleIds = new[] { 3, 4 }
            })
        };
        req.Headers.Add("x-test-perms", "ViewEmployees,CreateEmployees");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Created);
        var dto = await resp.Content.ReadFromJsonAsync<EmployeeDto>();
        dto!.Id.Should().Be(7);
    }

    [Fact]
    public async Task Create_403_WithoutPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Employees")
        {
            Content = JsonContent.Create(new { officeId = 1, legalEntityId = 2, email = "a@b", firstName = "A", lastName = "B", phone = "1", password = "x", leadLimit = 1, isDeleted = false, roleIds = Array.Empty<int>() })
        };
        req.Headers.Add("x-test-perms", "ViewEmployees");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
