using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthLoginBadTests(WebApplicationFactory<Program> f) : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task Login_Returns400_OnFail()
    {
        var (c, auth) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);

        auth.Setup(s => s.LoginAsync(It.IsAny<LoginDto>()))
            .ReturnsAsync(ServiceResult<TokenPairDto>.Fail("err"));

        var resp = await c.PostAsJsonAsync("/api/Auth/login", new LoginDto { Email = "x@x", Password = "bad" });

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}