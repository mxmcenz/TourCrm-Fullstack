using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Moq;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Permissions;
using TourCrm.Application.Interfaces;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Permissions;

public class PermissionsControllerTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    private static WebApplicationFactory<Program> BuildFactory(
        WebApplicationFactory<Program> baseFactory,
        Mock<IPermissionProvider> mockProvider)
    {
        return baseFactory.WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Testing");
            b.ConfigureServices(s =>
            {
                var hosted = s.Where(d => d.ServiceType == typeof(IHostedService)
                                          && d.ImplementationType?.Name == "PermissionsIntegrityHostedService")
                    .ToList();
                foreach (var d in hosted) s.Remove(d);

                var old = s.SingleOrDefault(d => d.ServiceType == typeof(IPermissionProvider));
                if (old != null) s.Remove(old);
                s.AddSingleton(mockProvider.Object);

                s.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = TestHeaderAuthHandler.Scheme;
                    o.DefaultChallengeScheme = TestHeaderAuthHandler.Scheme;
                }).AddScheme<AuthenticationSchemeOptions, TestHeaderAuthHandler>(TestHeaderAuthHandler.Scheme, _ => { });
            });
        });
    }

    [Fact]
    public async Task GetAll_200_Empty()
    {
        var mock = new Mock<IPermissionProvider>(MockBehavior.Strict);
        mock.Setup(p => p.GetPermissionsAsync()).ReturnsAsync(Array.Empty<PermissionDto>());

        var factory = BuildFactory(f, mock);
        var client = factory.CreateClient();

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Permissions");
        req.Headers.Add("x-test-userid", "1");
        var resp = await client.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<PermissionDto>>();
        list!.Should().BeEmpty();

        mock.Verify(p => p.GetPermissionsAsync(), Times.Once);
        mock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetAll_200_WithItems()
    {
        var items = new List<PermissionDto>
        {
            new() { Key = "ViewDictionaries", Name = "Просмотр справочников" },
            new() { Key = "EditLeads", Name = "Редактировать лиды" }
        };

        var mock = new Mock<IPermissionProvider>(MockBehavior.Strict);
        mock.Setup(p => p.GetPermissionsAsync()).ReturnsAsync(items);

        var factory = BuildFactory(f, mock);
        var client = factory.CreateClient();

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Permissions");
        req.Headers.Add("x-test-userid", "1");
        var resp = await client.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var list = await resp.Content.ReadFromJsonAsync<List<PermissionDto>>();
        list!.Should().HaveCount(2);
        list.Select(x => x.Key).Should().BeEquivalentTo("ViewDictionaries", "EditLeads");

        mock.Verify(p => p.GetPermissionsAsync(), Times.Once);
        mock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetTree_200_Empty()
    {
        var mock = new Mock<IPermissionProvider>(MockBehavior.Strict);
        mock.Setup(p => p.GetPermissionTreeAsync()).ReturnsAsync(Array.Empty<PermissionCategoryDto>());

        var factory = BuildFactory(f, mock);
        var client = factory.CreateClient();

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Permissions/tree");
        req.Headers.Add("x-test-userid", "1");
        var resp = await client.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var cats = await resp.Content.ReadFromJsonAsync<List<PermissionCategoryDto>>();
        cats!.Should().BeEmpty();

        mock.Verify(p => p.GetPermissionTreeAsync(), Times.Once);
        mock.VerifyNoOtherCalls();
    }

    [Fact]
    public async Task GetTree_200_WithItems()
    {
        var tree = new List<PermissionCategoryDto>
        {
            new()
            {
                Key = "Dictionaries",
                Name = "Справочники",
                Items = new List<PermissionDto>
                {
                    new() { Key = "ViewDictionaries", Name = "Просмотр" },
                    new() { Key = "EditDictionaries", Name = "Редактировать" }
                }
            }
        };

        var mock = new Mock<IPermissionProvider>(MockBehavior.Strict);
        mock.Setup(p => p.GetPermissionTreeAsync()).ReturnsAsync(tree);

        var factory = BuildFactory(f, mock);
        var client = factory.CreateClient();

        var req = new HttpRequestMessage(HttpMethod.Get, "/api/Permissions/tree");
        req.Headers.Add("x-test-userid", "1");
        var resp = await client.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        var cats = await resp.Content.ReadFromJsonAsync<List<PermissionCategoryDto>>();
        cats!.Should().HaveCount(1);
        cats[0].Items.Should().HaveCount(2);

        mock.Verify(p => p.GetPermissionTreeAsync(), Times.Once);
        mock.VerifyNoOtherCalls();
    }
}