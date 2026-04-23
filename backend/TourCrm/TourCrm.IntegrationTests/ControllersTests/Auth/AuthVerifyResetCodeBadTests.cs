using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthVerifyResetCodeBadTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task VerifyResetCode_Returns400_OnFail()
    {
        var (c, auth) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);
        auth.Setup(s => s.VerifyResetCodeAsync(It.IsAny<VerifyResetCodeDto>()))
            .ReturnsAsync(ServiceResult<object>.Fail("Неверный код"));

        var resp = await c.PostAsJsonAsync("/api/Auth/verify-reset-code", new VerifyResetCodeDto { UserId = 1, Code = "BAD" });
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}