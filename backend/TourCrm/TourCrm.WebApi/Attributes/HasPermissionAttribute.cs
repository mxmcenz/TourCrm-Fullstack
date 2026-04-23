using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TourCrm.WebApi.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class HasPermissionAttribute : Attribute, IAsyncAuthorizationFilter
{
    private readonly string[] _permissions;
    public bool RequireAll { get; set; } = false;

    public HasPermissionAttribute(params string[] permissions)
    {
        _permissions = permissions ?? Array.Empty<string>();
    }

    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var endpoint = context.HttpContext.GetEndpoint();
        if (endpoint?.Metadata.GetMetadata<IAllowAnonymous>() is not null)
            return Task.CompletedTask;

        var user = context.HttpContext.User;

        if (user?.Identity?.IsAuthenticated != true)
        {
            context.Result = new UnauthorizedResult();
            return Task.CompletedTask;
        }

        if (user.IsInRole("SuperAdmin"))
            return Task.CompletedTask;

        var perms = user.FindAll("permissions")
            .Select(c => c.Value)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var ok = _permissions.Length > 0 &&
                 (RequireAll ? _permissions.All(perms.Contains) : _permissions.Any(perms.Contains));

        if (!ok)
            context.Result = new ForbidResult();

        return Task.CompletedTask;
    }
}