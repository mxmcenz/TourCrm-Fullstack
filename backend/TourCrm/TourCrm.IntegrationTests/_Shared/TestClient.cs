using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using TourCrm.Application.Interfaces;

namespace TourCrm.IntegrationTests._Shared;

public static class TestClient
{
    public static HttpClient Create(WebApplicationFactory<Program> f) =>
        f.WithWebHostBuilder(b => b.UseEnvironment("Testing")).CreateClient();

    public static (HttpClient client, Mock<IAuthService> auth)
        CreateWithAuthMock(WebApplicationFactory<Program> f)
    {
        var mock = new Mock<IAuthService>(MockBehavior.Strict);

        var factory = f.WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Testing");
            b.ConfigureServices(s =>
            {
                var d = s.SingleOrDefault(x => x.ServiceType == typeof(IAuthService));
                if (d != null) s.Remove(d);
                s.AddSingleton(mock.Object);

                s.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = TestAuthHandler.Scheme;
                    o.DefaultChallengeScheme = TestAuthHandler.Scheme;
                }).AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.Scheme, _ => { });
            });
        });

        return (factory.CreateClient(), mock);
    }
    
    public static (HttpClient client, Mock<TService> mock)
        CreateWithHeaderAuthAndMock<TService>(WebApplicationFactory<Program> f)
        where TService : class
    {
        var mock = new Mock<TService>(MockBehavior.Strict);

        var factory = f.WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Testing");
            b.ConfigureServices(s =>
            {
                var d = s.SingleOrDefault(x => x.ServiceType == typeof(TService));
                if (d != null) s.Remove(d);
                s.AddSingleton(mock.Object);

                s.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = TestHeaderAuthHandler.Scheme;
                    o.DefaultChallengeScheme = TestHeaderAuthHandler.Scheme;
                }).AddScheme<AuthenticationSchemeOptions, TestHeaderAuthHandler>(TestHeaderAuthHandler.Scheme, _ => { });
            });
        });

        return (factory.CreateClient(), mock);
    }
    
    public static (HttpClient client, Mock<T1> mock1, Mock<T2> mock2)
        CreateWithHeaderAuthAndMocks<T1,T2>(WebApplicationFactory<Program> f)
        where T1 : class
        where T2 : class
    {
        var m1 = new Mock<T1>(MockBehavior.Strict);
        var m2 = new Mock<T2>(MockBehavior.Strict);

        var factory = f.WithWebHostBuilder(b =>
        {
            b.UseEnvironment("Testing");
            b.ConfigureServices(s =>
            {
                var d1 = s.SingleOrDefault(x => x.ServiceType == typeof(T1));
                if (d1 != null) s.Remove(d1);
                s.AddSingleton(m1.Object);

                var d2 = s.SingleOrDefault(x => x.ServiceType == typeof(T2));
                if (d2 != null) s.Remove(d2);
                s.AddSingleton(m2.Object);

                s.AddAuthentication(o =>
                {
                    o.DefaultAuthenticateScheme = TestHeaderAuthHandler.Scheme;
                    o.DefaultChallengeScheme = TestHeaderAuthHandler.Scheme;
                }).AddScheme<AuthenticationSchemeOptions, TestHeaderAuthHandler>(TestHeaderAuthHandler.Scheme, _ => { });
            });
        });

        return (factory.CreateClient(), m1, m2);
    }
}