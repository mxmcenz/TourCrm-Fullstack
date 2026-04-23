using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Moq;
using TourCrm.Application.Common.Results;
using TourCrm.Application.DTOs;
using TourCrm.Application.DTOs.Auths;

namespace TourCrm.IntegrationTests.ControllersTests.Auth;

public class AuthStartRegistrationOkTests(WebApplicationFactory<Program> f)
    : IClassFixture<WebApplicationFactory<Program>>
{
    [Fact]
    public async Task StartRegistration_Returns200()
    {
        var (c, auth) = TourCrm.IntegrationTests._Shared.TestClient.CreateWithAuthMock(f);

        var data = new { userId = 1, email = "u@test.local", firstName = "U", code = "1234" };
        auth.Setup(s => s.StartRegistrationAsync(It.IsAny<EmailPhoneNameDto>()))
            .ReturnsAsync(ServiceResult<object>.Ok(data, "Регистрация начата"));

        var resp = await c.PostAsJsonAsync("/api/Auth/start-registration",
            new EmailPhoneNameDto { Email = "u@test.local", PhoneNumber = "7707", FirstName = "U" });

        resp.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}