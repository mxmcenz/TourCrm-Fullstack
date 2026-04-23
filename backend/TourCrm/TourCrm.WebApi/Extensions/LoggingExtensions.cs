using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Sinks.Elasticsearch;

namespace TourCrm.WebApi.Extensions;

public static class LoggingExtensions
{
    public static void AddSerilogConfiguration(this ConfigureHostBuilder host)
    {
        host.UseSerilog((ctx, _, cfg) =>
        {
            var env = ctx.HostingEnvironment.EnvironmentName ?? "Production";
            var elasticUrl = ctx.Configuration["Elastic:Url"] ?? ctx.Configuration["Elastic__Url"];
            var indexPrefix = ctx.Configuration["Elastic:IndexPrefix"] ??
                              ctx.Configuration["Elastic__IndexPrefix"] ?? "tourcrm";
            var minLevelStr = ctx.Configuration["Serilog:MinimumLevel"] ?? ctx.Configuration["Serilog__MinimumLevel"];

            var lvl = Enum.TryParse(minLevelStr, out LogEventLevel parsed) ? parsed : LogEventLevel.Information;

            Serilog.Debugging.SelfLog.Enable(Console.Error);

            cfg.MinimumLevel.Is(lvl)
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.Hosting.Lifetime", LogEventLevel.Information)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .Enrich.WithProcessId()
                .Enrich.WithThreadId()
                .Enrich.WithExceptionDetails()
                .Enrich.WithProperty("app", "TourCrm.WebApi")
                .Enrich.WithProperty("env", env)
                .WriteTo.Console();

            if (!string.IsNullOrWhiteSpace(elasticUrl))
            {
                var es = new ElasticsearchSinkOptions(new Uri(elasticUrl))
                {
                    AutoRegisterTemplate = false,
                    IndexFormat = $"{indexPrefix}-logs-{env.ToLowerInvariant()}-{{0:yyyy.MM.dd}}",
                    EmitEventFailure = EmitEventFailureHandling.WriteToSelfLog
                };
                cfg.WriteTo.Elasticsearch(es);
            }
        });
    }
}