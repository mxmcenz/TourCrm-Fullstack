using System.Reflection;
using Microsoft.AspNetCore.Mvc;
using TourCrm.Application.Interfaces;

namespace TourCrm.WebApi.Startup;

public sealed class PermissionsIntegrityHostedService(
    IPermissionProvider provider,
    ILogger<PermissionsIntegrityHostedService> logger
) : IHostedService
{
    public async Task StartAsync(CancellationToken ct)
    {
        var known = (await provider.GetPermissionsAsync())
            .Select(p => p.Key)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var required = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var attrFullName = "TourCrm.WebApi.Attributes.HasPermissionAttribute";

        foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
        {
            Type[] types;
            try
            {
                types = asm.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null)!.Cast<Type>().ToArray();
            }

            foreach (var t in types.Where(t =>
                         t != null &&
                         (typeof(ControllerBase).IsAssignableFrom(t) ||
                          t.GetCustomAttributes(typeof(ApiControllerAttribute), inherit: true).Any())))
            {
                Collect(t, required, attrFullName);

                foreach (var m in t.GetMethods(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly))
                    Collect(m, required, attrFullName);
            }
        }

        var missing = required.Where(k => !known.Contains(k)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        if (missing.Count > 0)
            throw new InvalidOperationException("Unknown permissions in attributes: " + string.Join(", ", missing));

        logger.LogInformation("Permissions check OK. Known={Known} Used={Used}", known.Count, required.Count);
    }

    public Task StopAsync(CancellationToken ct) => Task.CompletedTask;

    static void Collect(MemberInfo member, HashSet<string> sink, string attrFullName)
    {
        foreach (var cad in CustomAttributeData.GetCustomAttributes(member))
        {
            if (!string.Equals(cad.AttributeType.FullName, attrFullName, StringComparison.Ordinal)) continue;
            if (cad.ConstructorArguments.Count != 1) continue;
            var arg = cad.ConstructorArguments[0];
            if (arg.Value is IReadOnlyList<CustomAttributeTypedArgument> arr)
                foreach (var v in arr)
                    if (v.Value is string s && !string.IsNullOrWhiteSpace(s))
                        sink.Add(s);
        }
    }
}