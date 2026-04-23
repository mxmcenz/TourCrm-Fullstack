using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthVerifyResetCodeOkTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task VerifyResetCode_Returns200()
    {
        var (c, auth) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);
        auth.Setup(s => s.VerifyResetCodeAsync(It.IsAny<VerifyResetCodeDto>()))
            .ReturnsAsync(ServiceResult<object>.Ok(new { userId = 1, email = "u@test.local" }, "OK"));

        var resp = await c.PostAsJsonAsync("/api/Auth/verify-reset-code", new VerifyResetCodeDto { UserId = 1, Code = "7777" });
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}