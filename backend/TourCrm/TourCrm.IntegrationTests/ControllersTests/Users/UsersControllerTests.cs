using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Users;

public class UsersControllerTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task GetUsers_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IUserService>(f);

        svc.Setup(s => s.GetAllAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<List<UserDto>>.Ok(new List<UserDto>
            {
                new() { Id = 1 },
                new() { Id = 2 }
            }));

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Users");
        req.Headers.Add("x-test-role", "SuperAdmin");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var list = await resp.Content.ReadFromJsonAsync<List<UserDto>>();
        list!.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetUser_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IUserService>(f);

        svc.Setup(s => s.GetByIdAsync(5, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<UserDto>.Ok(new UserDto { Id = 5 }));

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Users/5");
        req.Headers.Add("x-test-role", "SuperAdmin");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var dto = await resp.Content.ReadFromJsonAsync<UserDto>();
        dto!.Id.Should().Be(5);
    }

    [Fact]
    public async Task GetUser_404()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IUserService>(f);

        svc.Setup(s => s.GetByIdAsync(42, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<UserDto>.Fail("not found"));

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Users/42");
        req.Headers.Add("x-test-role", "SuperAdmin");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_201()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IUserService>(f);

        svc.Setup(s => s.CreateAsync(It.IsAny<CreateUserDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<int>.Ok(77));

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Users")
        {
            Content = JsonContent.Create(new CreateUserDto { Email = "new@ex.com", FirstName = "N" })
        };
        req.Headers.Add("x-test-role", "SuperAdmin");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Created);

        var id = await resp.Content.ReadFromJsonAsync<int>();
        id.Should().Be(77);
    }

    [Fact]
    public async Task Update_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IUserService>(f);

        svc.Setup(s => s.UpdateAsync(7, It.IsAny<UpdateUserDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<string>.Ok("updated"));

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Users/7")
        {
            Content = JsonContent.Create(new UpdateUserDto { FirstName = "Upd" })
        };
        req.Headers.Add("x-test-role", "SuperAdmin");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Delete_200()
    {
        var (c, svc) = TestClient.CreateWithHeaderAuthAndMock<IUserService>(f);

        svc.Setup(s => s.DeleteAsync(9, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<string>.Ok("deleted"));

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/Users/9");
        req.Headers.Add("x-test-role", "SuperAdmin");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Forbidden_403_WhenNotSuperAdmin()
    {
        var (c, _) = TestClient.CreateWithHeaderAuthAndMock<IUserService>(f);

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Users");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.Forbidden);
    }
}