using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthSetPasswordOkTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task SetPassword_Returns200_AndSetsCookies()
    {
        var (c, auth) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);

        var tokens = new TokenPairDto
        {
            AccessToken = "jwt", AccessTokenExpiresIn = 15,
            RefreshToken = "rt", RefreshTokenExpiresIn = 30,
            User = new UserStateDto { UserId = 1, Email = "u@test.local", IsEmailConfirmed = true }
        };

        auth.Setup(s => s.SetPasswordAsync(It.IsAny<SetPasswordDto>()))
            .ReturnsAsync(ServiceResult<TokenPairDto>.Ok(tokens, "OK"));

        var resp = await c.PostAsJsonAsync("/api/Auth/set-password",
            new SetPasswordDto { UserId = 1, Password = "P@ssw0rd", ConfirmPassword = "P@ssw0rd" });

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        resp.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        cookies!.Should().Contain(x => x.StartsWith("jwt="));
        cookies.Should().Contain(x => x.StartsWith("refresh-token="));
    }
}