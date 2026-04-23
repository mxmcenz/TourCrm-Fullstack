using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthUpdateProfileBadTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task UpdateProfile_Returns400_OnFail()
    {
        var (c, auth) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);
        auth.Setup(s => s.UpdateProfileAsync(It.IsAny<System.Security.Claims.ClaimsPrincipal>(), It.IsAny<UpdateProfileDto>()))
            .ReturnsAsync(ServiceResult<object>.Fail("Пользователь не найден"));

        var resp = await c.PutAsJsonAsync("/api/Auth/update-profile", new UpdateProfileDto { PhoneNumber = "7707" });
        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}