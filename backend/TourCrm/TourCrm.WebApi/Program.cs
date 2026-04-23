using Microsoft.AspNetCore.HttpOverrides;
using Serilog;
using TourCrm.Application;
using TourCrm.Infrastructure;
using TourCrm.Infrastructure.Data;
using TourCrm.WebApi.Extensions;
using TourCrm.WebApi.Middlewares;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddSerilogConfiguration();

builder.Services.AddWebServices(); 
builder.Services.AddPermissionServices();
builder.Services.AddSwaggerDocumentation();
builder.Services.AddIdentityServices(builder.Configuration);
builder.Services.AddAppCors(builder.Configuration);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var initializer = scope.ServiceProvider.GetRequiredService<DbInitializer>();
    await initializer.InitializeAsync();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto | ForwardedHeaders.XForwardedFor
});

app.UseCors("AllowFrontend");

app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = (diag, http) =>
    {
        diag.Set("Path", http.Request.Path);
        diag.Set("Method", http.Request.Method);
        diag.Set("StatusCode", http.Response.StatusCode);
    };
});

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.UseLoggingContext();

app.MapControllers();

app.MapGet("/", () => Results.Text("OK"));
app.MapGet("/healthz", () => Results.Ok(new { status = "healthy" }));

if (app.Environment.IsEnvironment("Testing"))
{
    app.MapGet("/__throw", _ => throw new Exception("boom"));
}

app.Run();

public partial class Program { }