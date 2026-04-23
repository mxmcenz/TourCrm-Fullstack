using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Permissions;
using TourCrm.Application.DTOs.Roles;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Roles;

public class RolesControllerTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    private static List<PermissionDto> AllPerms() =>
    [
        new() { Key = "ViewDictionaries", Name = "ViewDictionaries" },
        new() { Key = "CreateDictionaries", Name = "CreateDictionaries" },
        new() { Key = "EditDictionaries", Name = "EditDictionaries" },
        new() { Key = "DeleteDictionaries", Name = "DeleteDictionaries" },
        new() { Key = "ViewClients", Name = "ViewClients" },
        new() { Key = "CreateClients", Name = "CreateClients" },
        new() { Key = "EditClients", Name = "EditClients" },
        new() { Key = "DeleteClients", Name = "DeleteClients" },
        new() { Key = "ViewDeals", Name = "ViewDeals" },
        new() { Key = "CreateDeals", Name = "CreateDeals" },
        new() { Key = "EditDeals", Name = "EditDeals" },
        new() { Key = "ViewEmployees", Name = "ViewEmployees" },
        new() { Key = "CreateEmployees", Name = "CreateEmployees" },
        new() { Key = "EditEmployees", Name = "EditEmployees" },
        new() { Key = "DeleteEmployees", Name = "DeleteEmployees" },
        new() { Key = "ViewLeads", Name = "ViewLeads" },
        new() { Key = "CreateLeads", Name = "CreateLeads" },
        new() { Key = "EditLeads", Name = "EditLeads" },
        new() { Key = "DeleteLeads", Name = "DeleteLeads" },
        new() { Key = "ViewLegalEntities", Name = "ViewLegalEntities" },
        new() { Key = "CreateLegalEntities", Name = "CreateLegalEntities" },
        new() { Key = "EditLegalEntities", Name = "EditLegalEntities" },
        new() { Key = "DeleteLegalEntities", Name = "DeleteLegalEntities" },
        new() { Key = "ViewOffices", Name = "ViewOffices" },
        new() { Key = "CreateOffices", Name = "CreateOffices" },
        new() { Key = "EditOffices", Name = "EditOffices" },
        new() { Key = "DeleteOffices", Name = "DeleteOffices" },
        new() { Key = "ViewRoles", Name = "ViewRoles" },
        new() { Key = "CreateRoles", Name = "CreateRoles" },
        new() { Key = "EditRoles", Name = "EditRoles" },
        new() { Key = "DeleteRoles", Name = "DeleteRoles" }
    ];

    private static (HttpClient c, Mock<IRoleService> svc, Mock<IPermissionProvider> perms)
        MakeClient(WebApplicationFactory<Program> f)
    {
        var svc = new Mock<IRoleService>(MockBehavior.Strict);
        var perms = new Mock<IPermissionProvider>(MockBehavior.Strict);

        perms.Setup(p => p.GetPermissionsAsync()).ReturnsAsync(AllPerms());
        perms.Setup(p => p.GetPermissionTreeAsync())
            .ReturnsAsync(new List<PermissionCategoryDto>
            {
                new() { Key = "General", Name = "General", Items = AllPerms() }
            });

        var factory = f.WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Testing");
            b.ConfigureServices(s =>
            {
                var d1 = s.SingleOrDefault(x => x.ServiceType == typeof(IRoleService));
                if (d1 != null) s.Remove(d1);
                s.AddSingleton(svc.Object);

                var d2 = s.SingleOrDefault(x => x.ServiceType == typeof(IPermissionProvider));
                if (d2 != null) s.Remove(d2);
                s.AddSingleton(perms.Object);

                s.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = TestHeaderAuthHandler.Scheme;
                    o.DefaultChallengeScheme = TestHeaderAuthHandler.Scheme;
                }).AddScheme<AuthenticationSchemeOptions, TestHeaderAuthHandler>(
                    TestHeaderAuthHandler.Scheme, _ => { });
            });
        });

        var client = factory.CreateClient();
        return (client, svc, perms);
    }

    [Fact]
    public async Task GetAll_200()
    {
        var (c, svc, _) = MakeClient(f);
        svc.Setup(s => s.GetAllRolesAsync("1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<List<RoleDto>>
                { Success = true, Data = new() { new RoleDto { Id = 1, Name = "Admin" } } });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Roles");
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewRoles");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<RoleDto>>();
        list!.Should().HaveCount(1);
        list[0].Name.Should().Be("Admin");
    }

    [Fact]
    public async Task GetById_404()
    {
        var (c, svc, _) = MakeClient(f);
        svc.Setup(s => s.GetRoleByIdAsync(99, "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<RoleDto> { Success = false, Message = "not found" });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Roles/99");
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewRoles");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_201()
    {
        var (c, svc, _) = MakeClient(f);
        var created = new RoleDto { Id = 42, Name = "Manager" };
        svc.Setup(s => s.CreateRoleAsync(It.IsAny<CreateRoleDto>(), "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<RoleDto> { Success = true, Data = created });

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Roles")
        {
            Content = JsonContent.Create(new CreateRoleDto { Name = "Manager" })
        };
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewRoles,CreateRoles");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.Created);

        var dto = await resp.Content.ReadFromJsonAsync<RoleDto>();
        dto!.Id.Should().Be(42);
    }

    [Fact]
    public async Task Update_204()
    {
        var (c, svc, _) = MakeClient(f);

        svc.Setup(s => s.GetRoleByIdAsync(5, "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<RoleDto?>.Ok(new RoleDto { Id = 5, Name = "Old" }));

        svc.Setup(s => s.UpdateRoleAsync(It.Is<UpdateRoleDto>(d => d.Id == 5), "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<object>.Ok("updated"));

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Roles")
        {
            Content = JsonContent.Create(new UpdateRoleDto { Id = 5, Name = "New" })
        };
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewRoles,EditRoles");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task Delete_204()
    {
        var (c, svc, _) = MakeClient(f);
        svc.Setup(s => s.DeleteRoleAsync(7, "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<object>.Ok("deleted"));

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/Roles/7");
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewRoles,DeleteRoles");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.NoContent);
    }

    [Fact]
    public async Task GetPermissions_200()
    {
        var (c, svc, _) = MakeClient(f);
        svc.Setup(s => s.GetRolePermissionsAsync(3, "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<List<RolePermissionDto>>.Ok(
                new List<RolePermissionDto> { new() { Key = "ViewRoles", Name = "View Roles", IsGranted = true } }
            ));

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Roles/3/permissions");
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewRoles");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task UpdatePermissions_200()
    {
        var (c, svc, _) = MakeClient(f);
        svc.Setup(
                s => s.SetRolePermissionsAsync(3, "1", It.IsAny<GrantPermissionsDto>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<object>.Ok("ok"));

        var req = new HttpRequestMessage(HttpMethod.Put, "/api/Roles/3/permissions")
        {
            Content = JsonContent.Create(new GrantPermissionsDto { Keys = new() { "ViewRoles" } })
        };
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewRoles,EditRoles");

        var resp = await c.SendAsync(req);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task AssignRoleToUser_200()
    {
        var (c, svc, _) = MakeClient(f);
        svc.Setup(s => s.AssignRoleToUserAsync(10, 2, "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<object>.Ok("assigned"));

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Roles/assign?userId=10&roleId=2");
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewRoles,EditRoles");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task RemoveRoleFromUser_200()
    {
        var (c, svc, _) = MakeClient(f);
        svc.Setup(s => s.RemoveRoleFromUserAsync(10, 2, "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceResult<object>.Ok("removed"));

        var req = new HttpRequestMessage(HttpMethod.Delete, "/api/Roles/remove?userId=10&roleId=2");
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewRoles,EditRoles");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetUserRoles_200()
    {
        var (c, svc, _) = MakeClient(f);
        svc.Setup(s => s.GetUserRolesAsync(10, "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<List<RoleDto>>
                { Success = true, Data = new() { new RoleDto { Id = 1, Name = "R" } } });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Roles/user/10");
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewRoles");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task GetPaged_200()
    {
        var (c, svc, _) = MakeClient(f);
        var paged = new PagedResult<RoleDto>
            { Items = new() { new RoleDto { Id = 3, Name = "M" } }, TotalCount = 1, Page = 1, PageSize = 10 };
        svc.Setup(s => s.GetRolesPagedAsync(It.IsAny<RolesQuery>(), "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<PagedResult<RoleDto>> { Success = true, Data = paged });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Roles/paged?page=1&pageSize=10");
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewRoles");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }

    [Fact]
    public async Task Suggest_200()
    {
        var (c, svc, _) = MakeClient(f);
        var paged = new PagedResult<RoleDto>
            { Items = new() { new RoleDto { Id = 7, Name = "Manager" } }, TotalCount = 1, Page = 1, PageSize = 10 };
        svc.Setup(s => s.GetRolesPagedAsync(It.IsAny<RolesQuery>(), "1", It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ServiceResult<PagedResult<RoleDto>> { Success = true, Data = paged });

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Roles/suggest?term=man&take=5");
        req.Headers.Add("x-test-userid", "1");
        req.Headers.Add("x-test-perms", "ViewRoles");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var names = await resp.Content.ReadFromJsonAsync<List<string>>();
        names!.Should().ContainSingle("Manager");
    }
}