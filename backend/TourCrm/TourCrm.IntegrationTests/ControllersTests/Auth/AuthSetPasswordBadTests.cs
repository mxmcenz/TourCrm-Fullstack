using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthSetPasswordBadTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task SetPassword_Returns400_OnFail()
    {
        var (c, auth) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);
        auth.Setup(s => s.SetPasswordAsync(It.IsAny<SetPasswordDto>()))
            .ReturnsAsync(ServiceResult<TokenPairDto>.Fail("Пароли не совпадают"));

        var resp = await c.PostAsJsonAsync("/api/Auth/set-password",
            new SetPasswordDto { UserId = 1, Password = "A", ConfirmPassword = "B" });

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}