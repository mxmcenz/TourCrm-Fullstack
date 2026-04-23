using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthResendResetCodeOkTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task ResendResetCode_Returns200()
    {
        var (c, auth) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);
        auth.Setup(s => s.ResendResetCodeAsync(It.IsAny<EmailDto>()))
            .ReturnsAsync(ServiceResult<object>.Ok(new { userId = 1, email = "u@test.local", code = "5678" }, "OK"));

        var resp = await c.PostAsJsonAsync("/api/Auth/resend-reset-code", new EmailDto { Email = "u@test.local" });
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}