using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Employees;

public class EmployeesDeleteRestoreTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task SoftDelete_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        svc.Setup(s => s.MarkAsDeletedAsync(5, "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(true);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/Employees/5");
        req.Headers.Add("x-test-perms", "ViewEmployees,DeleteEmployees");
        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task SoftDelete_404_WhenFalse()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        svc.Setup(s => s.MarkAsDeletedAsync(99, "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(false);

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/Employees/99");
        req.Headers.Add("x-test-perms", "ViewEmployees,DeleteEmployees");
        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Restore_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        svc.Setup(s => s.RestoreAsync(7, "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(true);

        var req = new HttpRequestMessage(HttpMethod.Patch, "/api/Employees/7/restore");
        req.Headers.Add("x-test-perms", "ViewEmployees,EditEmployees");
        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Restore_404_WhenFalse()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        svc.Setup(s => s.RestoreAsync(77, "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(false);

        var req = new HttpRequestMessage(HttpMethod.Patch, "/api/Employees/77/restore");
        req.Headers.Add("x-test-perms", "ViewEmployees,EditEmployees");
        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}
