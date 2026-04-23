using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthResetPasswordBadTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task ResetPassword_Returns400_OnFail()
    {
        var (c, auth) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);
        auth.Setup(s => s.ResetPasswordAsync(It.IsAny<SetPasswordDto>()))
            .ReturnsAsync(ServiceResult<TokenPairDto>.Fail("Сначала запросите код сброса"));

        var resp = await c.PostAsJsonAsync("/api/Auth/reset-password",
            new SetPasswordDto { UserId = 1, Password = "X", ConfirmPassword = "X" });

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}