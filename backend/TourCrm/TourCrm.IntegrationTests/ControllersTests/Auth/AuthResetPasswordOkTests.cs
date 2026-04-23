// ControllersTests/Auth/AuthResetPasswordOkTests.cs

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthResetPasswordOkTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task ResetPassword_Returns200_AndSetsCookies()
    {
        var (c, auth) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);

        var tokens = new TokenPairDto
        {
            AccessToken = "jwt2", AccessTokenExpiresIn = 15,
            RefreshToken = "rt2", RefreshTokenExpiresIn = 30,
            User = new UserStateDto { UserId = 1, Email = "u@test.local", IsEmailConfirmed = true }
        };

        auth.Setup(s => s.ResetPasswordAsync(It.IsAny<SetPasswordDto>()))
            .ReturnsAsync(ServiceResult<TokenPairDto>.Ok(tokens, "OK"));

        var resp = await c.PostAsJsonAsync("/api/Auth/reset-password",
            new SetPasswordDto { UserId = 1, Password = "NewP@ss", ConfirmPassword = "NewP@ss" });

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
        resp.Headers.TryGetValues("Set-Cookie", out var cookies).Should().BeTrue();
        cookies!.Should().Contain(x => x.StartsWith("jwt="));
        cookies.Should().Contain(x => x.StartsWith("refresh-token="));
    }
}