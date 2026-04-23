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

public class EmployeesUpdateTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Update_204()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        svc.Setup(s => s.UpdateAsync(5, It.IsAny<EmployeeUpdateDto>(), "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(true);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Employees/5")
        {
            Content = JsonContent.Create(new
            {
                officeId = 1,
                legalEntityId = 2,
                email = "edit@user.local",
                firstName = "Edit",
                lastName = "User",
                phone = "+222",
                leadLimit = 5,
                isDeleted = false,
                roleIds = new[] { 3 }
            })
        };
        req.Headers.Add("x-test-perms", "ViewEmployees,EditEmployees");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Update_404_WhenServiceReturnsFalse()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);
        svc.Setup(s => s.UpdateAsync(99, It.IsAny<EmployeeUpdateDto>(), "1", It.IsAny<CancellationToken>()))
           .ReturnsAsync(false);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Employees/99")
        {
            Content = JsonContent.Create(new
            {
                officeId = 1,
                legalEntityId = 2,
                email = "x@y",
                firstName = "X",
                lastName = "Y",
                phone = "1",
                leadLimit = 1,
                isDeleted = false,
                roleIds = Array.Empty<int>()
            })
        };
        req.Headers.Add("x-test-perms", "ViewEmployees,EditEmployees");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Update_403_WithoutPermission()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IEmployeeService>(f);

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Employees/5")
        {
            Content = JsonContent.Create(new
            {
                officeId = 1,
                legalEntityId = 2,
                email = "no@perm",
                firstName = "No",
                lastName = "Perm",
                phone = "1",
                leadLimit = 1,
                isDeleted = false,
                roleIds = Array.Empty<int>()
            })
        };
        req.Headers.Add("x-test-perms", "ViewEmployees");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}
