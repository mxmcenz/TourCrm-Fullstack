using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthUpdateProfileOkTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task UpdateProfile_Returns200()
    {
        var (c, auth) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);
        auth.Setup(s => s.UpdateProfileAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), It.IsAny<UpdateProfileDto>()))
            .ReturnsAsync(ServiceResult<object>.Ok(new { userId = 1, email = "u@test.local" }, "OK"));

        var resp = await c.PutAsJsonAsync("/api/Auth/update-profile", new UpdateProfileDto { PhoneNumber = "7707" });
        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}