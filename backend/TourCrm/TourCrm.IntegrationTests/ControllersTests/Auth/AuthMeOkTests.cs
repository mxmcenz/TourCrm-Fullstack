using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Core.Entities;
using TourCrm.IntegrationTests._Shared;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthMeOkTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Me_ReturnsUserProjection()
    {
        var (c, auth) = TestClient.CreateWithAuthMock(f);

        auth.Setup(s => s.GetCurrentUserAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>()))
            .ReturnsAsync(new User { Id = 1, Email = "user@test.local", IsEmailConfirmed = true });

        var resp = await c.GetAsync("/api/Auth/me");
        resp.StatusCode.Should().Be(HttpStatusCode.OK);

        var json = await resp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        json!.Should().ContainKey("email");
        json["email"].ToString().Should().Be("user@test.local");
        json.Should().ContainKey("roles");
        json.Should().ContainKey("permissions");
        json.Should().ContainKey("isSuperAdmin");
        json.Should().ContainKey("companyId");
    }
}