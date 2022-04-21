using System.Security.Claims;
using Microsoft.AspNetCore.Builder;

namespace BattlelineUtilities.AspNetCore.Auth;

public static class WebApplicationExtensions
{
    /// <summary>
    ///     When using custom claims in Auth Rules this will allow you to remap an email claim.  This must
    ///     be called after UseAuthentication and before UseAuthorization.
    /// </summary>
    /// <param name="app">
    ///     <see cref="WebApplication" />
    /// </param>
    /// <param name="name">Custom Claim Name</param>
    public static WebApplication UseCustomEmailClaim(this WebApplication app, string name)
    {
        app.Use(async (context, next) =>
        {
            var userIdentity = context.User.Identities.FirstOrDefault();

            userIdentity?.AddClaim(new Claim(ClaimTypes.Email,
                userIdentity.Claims.FirstOrDefault(c => c.Type == name)?.Value ?? "unknown"));

            await next.Invoke();
        });

        return app;
    }

    /// <summary>
    ///     When using custom claims in Auth Rules this will allow you to remap an email claim.  This must
    ///     be called after UseAuthentication and before UseAuthorization.
    /// </summary>
    /// <param name="app">
    ///     <see cref="WebApplication" />
    /// </param>
    /// <param name="mapTo">value to map the claim to</param>
    /// <param name="mapFrom">search key claim is being mapped from</param>
    public static WebApplication UseCustomClaim(this WebApplication app, string mapTo, string mapFrom)
    {
        app.Use(async (context, next) =>
        {
            var userIdentity = context.User.Identities.FirstOrDefault();

            userIdentity?.AddClaim(new Claim(mapTo,
                userIdentity.Claims.FirstOrDefault(c => c.Type == mapFrom)?.Value ?? "unknown"));

            await next.Invoke();
        });

        return app;
    }

    /// <summary>
    ///     This will instruct your application to add roles claims to the default identity for each request.  This
    ///     is required to allow for Role based Authorization.  This call must be made before calling UseAuthorization
    ///     in the pipeline.  It also requires that the Services Collection contain a valid <see cref="IRoleValidator" />
    ///     instance.
    /// </summary>
    /// <param name="app">
    ///     <see cref="WebApplication" />
    /// </param>
    /// <param name="roles">List of Application Roles</param>
    public static WebApplication UseRoles(this WebApplication app, List<string> roles)
    {
        app.Use(async (context, next) =>
        {
            var userIdentity = context.User.Identities.FirstOrDefault();

            if (userIdentity != null)
            {
                var roleValidator =
                    (IRoleValidator) context.RequestServices.GetService(typeof(IRoleValidator))!;

                foreach (var role in roles.Where(role => roleValidator.UserHasRole(userIdentity, role)))
                    userIdentity.AddClaim(new Claim(ClaimTypes.Role, role));
            }

            await next.Invoke();
        });

        return app;
    }

    /// <summary>
    ///     This will instruct your application to add permission claims to the default identity for each request.  This
    ///     is required to allow for Permission Based Authorization.  This call must be made before calling UseAuthorization
    ///     in the ASP.Net pipeline.  It also requires that the Services Collection contain a valid
    ///     <see cref="IRoleValidator" />
    ///     instance.
    /// </summary>
    /// <param name="app">
    ///     <see cref="WebApplication" />
    /// </param>
    /// <param name="permissions">List of Application Permissions</param>
    public static WebApplication UsePermissions(this WebApplication app, List<string> permissions)
    {
        app.Use(async (context, next) =>
        {
            var userIdentity = context.User.Identities.FirstOrDefault();

            if (userIdentity != null)
            {
                var permissionValidator =
                    (IPermissionValidator) context.RequestServices.GetService(typeof(IPermissionValidator))!;

                foreach (var permission in permissions.Where(permission =>
                             permissionValidator.UserHasPermission(userIdentity, permission)))
                    userIdentity.AddClaim(new Claim("permission", permission));
            }

            await next.Invoke();
        });

        return app;
    }
}
