using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthLogoutTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Logout_DeletesCookies_AndReturns200()
    {
        var (c, _) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);
        var resp = await c.PostAsync("/api/Auth/logout", content: null);
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        resp.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        cookies!.Should().Contain(x => x.StartsWith("jwt="));
        cookies.Should().Contain(x => x.StartsWith("refresh-token="));
    }
}