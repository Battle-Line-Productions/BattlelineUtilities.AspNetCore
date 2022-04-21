using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace BattlelineUtilities.AspNetCore.Auth;

public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     This method will configure the services collection with the necessary setup details for authentication
    ///     using any object inheriting from IAuthConfig.
    /// </summary>
    /// <param name="services">Services Collection</param>
    /// <param name="config"><see cref="IAuthConfig" />Any object inheriting IAuthConfig</param>
    public static IServiceCollection AddAuthenticationWithAuthConfig(this IServiceCollection services,
        IAuthConfig config)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Authority = config.Authority;
                options.Audience = config.Audience;
                options.RequireHttpsMetadata = false;

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        if (context.SecurityToken is not JwtSecurityToken token) return Task.CompletedTask;
                        if (context.Principal?.Identity is ClaimsIdentity identity)
                            identity.AddClaim(new Claim("access_token", token.RawData));

                        return Task.CompletedTask;
                    }
                };
            });

        return services;
    }

    /// <summary>
    ///     This will add policies that check for claims to ensure that certain roles are present.  It allows the
    ///     use of policies that check for multiple roles.
    /// </summary>
    /// <param name="services">Services Collection</param>
    /// <param name="rolePolicies">Key = Policy Name, and Value = List of Role Names</param>
    public static IServiceCollection AddRoleBasedAuthorizationWithRoles(this IServiceCollection services,
        Dictionary<string, List<string>> rolePolicies)
    {
        services.AddAuthorization(options =>
        {
            foreach (var rolePolicy in rolePolicies)
                options.AddPolicy(rolePolicy.Key, policy => policy.RequireClaim(ClaimTypes.Role, rolePolicy.Value));
        });

        return services;
    }

    /// <summary>
    ///     This will add policies that check for specific permissions in the identities claim.  You can then specify
    ///     the permission required for each action in your application.
    /// </summary>
    /// <param name="services">Services Collection</param>
    /// <param name="permissions">List of Permissions</param>
    public static IServiceCollection AddPermissionBasedAuthorizationWithPermissions(this IServiceCollection services,
        List<string> permissions)
    {
        services.AddAuthorization(options =>
        {
            foreach (var permission in permissions)
                options.AddPolicy(permission, policy => policy.RequireClaim("permission", permission));
        });

        return services;
    }
}
