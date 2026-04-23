using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Employees;

public class EmployeesGeneratePasswordTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GeneratePassword_200_WithPermission()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        svc.Setup(s => s.GeneratePasswordAsync(12))
            .ReturnsAsync("Abcd1234!@#$");

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Employees/generate-password?length=12");
        req.Headers.Add("x-test-perms", "ViewEmployees,CreateEmployees");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var json = await resp.Content.ReadFromJsonAsync<Dictionary<string,string>>();
        json!["password"].Should().Be("Abcd1234!@#$");
    }

    [Fact]
    public async Task GeneratePassword_403_NoPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Employees/generate-password?length=8");
        req.Headers.Add("x-test-perms", "ViewEmployees");
        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}