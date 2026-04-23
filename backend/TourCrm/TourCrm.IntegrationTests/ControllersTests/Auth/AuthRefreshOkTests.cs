using System.Net;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthRefreshOkTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Refresh_WithCookie_IssuesNewCookies()
    {
        var (c, auth) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);

        var tokens = new TokenPairDto
        {
            AccessToken = "new-jwt", AccessTokenExpiresIn = 15,
            RefreshToken = "new-rt", RefreshTokenExpiresIn = 30,
            User = new UserStateDto { UserId = 1, Email = "user@test.local", IsEmailConfirmed = true }
        };

        auth.Setup(s => s.RefreshTokenAsync("old-rt"))
            .ReturnsAsync(ServiceResult<TokenPairDto>.Ok(tokens, "ok"));

        var req = new HttpRequestMessage(HttpMethod.Post, "/api/Auth/refresh");
        req.Headers.Add("Cookie", "refresh-token=old-rt");
        var resp = await c.SendAsync(req);

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        resp.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        cookies!.Should().Contain(x => x.StartsWith("jwt="));
        cookies.Should().Contain(x => x.StartsWith("refresh-token="));
    }
}