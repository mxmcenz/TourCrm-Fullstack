using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthStartRegistrationBadTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task StartRegistration_Returns400_OnFail()
    {
        var (c, auth) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);
        auth.Setup(s => s.StartRegistrationAsync(It.IsAny<EmailPhoneNameDto>()))
            .ReturnsAsync(ServiceResult<object>.Fail("Email уже зарегистрирован"));

        var resp = await c.PostAsJsonAsync("/api/Auth/start-registration",
            new EmailPhoneNameDto { Email = "u@test.local", PhoneNumber = "7707", FirstName = "U" });

        resp.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }
}