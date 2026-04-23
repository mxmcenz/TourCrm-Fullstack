using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthVerifyEmailOkTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task VerifyEmail_Returns200()
    {
        var (c, auth) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);
        auth.Setup(s => s.VerifyEmailAsync(It.IsAny<VerifyEmailByCodeDto>()))
            .ReturnsAsync(ServiceResult<object>.Ok(new { userId = 1, email = "u@test.local", isEmailConfirmed = true }, "OK"));

        var resp = await c.PostAsJsonAsync("/api/Auth/verify-email", new VerifyEmailByCodeDto { UserId = 1, Code = "1234" });
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}