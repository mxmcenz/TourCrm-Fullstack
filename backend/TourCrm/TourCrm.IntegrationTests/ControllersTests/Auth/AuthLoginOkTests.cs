using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthLoginOkTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Login_SetsCookies_AndReturns200()
    {
        var (c, auth) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);

        var tokens = new TokenPairDto
        {
            AccessToken = "jwt", AccessTokenExpiresIn = 15,
            RefreshToken = "rt", RefreshTokenExpiresIn = 30,
            User = new UserStateDto { UserId = 1, Email = "user@test.local", IsEmailConfirmed = true }
        };

        auth.Setup(s => s.LoginAsync(It.IsAny<LoginDto>()))
            .ReturnsAsync(ServiceResult<TokenPairDto>.Ok(tokens, "ok"));

        var resp = await c.PostAsJsonAsync("/api/Auth/login", new LoginDto { Email = "user@test.local", Password = "p" });

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        resp.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        cookies!.Should().Contain(x => x.StartsWith("jwt="));
        cookies.Should().Contain(x => x.StartsWith("refresh-token="));
    }
}