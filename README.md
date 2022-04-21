# BattlelineUtilities.AspNetCore

## Auth:

This library is designed to allow the developer to easily configure Auth for advanced authorization scinarios in 
.Net 6 WebAPIs.

### Getting Started

1. Install Nuget Package
2. Create your AuthConfig class that inherits from IAuthConfig
3. use services.AddAuthenticationWithAuthConfig(AuthConfig); to add Auth to your application.
4. use services.AddPermissionBasedAuthorizationWithPermissions(List of Permissions); to add Permission based authorization or services.AddRoleBasedAuthorizationWithRoles(List of Role Policies); in to add Role based authorization.
5. Add IRoleValidator or IPermissionsValidator to services collection to enable Roles or Permission based on use case. You can override existing UserServices or create your own
6. In your Application Pipeline add UseAthentication.
7. In your Application Pipeline add UsePermissions or UseRoles based on your needs.
8. In your Application Pipeline add UseAuthorization to enable the use of policies in the Authorize attribute.

```csharp
// Program.cs example

var builder = WebApplication.CreateBuilder(args);
IConfiguration configuration = builder.Configuration;

var authConfig = new AuthConfig();

configuration.GetSection("Auth").Bind(authConfig);

var services = builder.Services;

services.AddController();
services.AddAuthenticationWithAuthConfig(authConfig);
services.AddScoped<UserService>();
services.AddScoped<IRoleValidator, UserService>();

var provider = services.BuildServiceProvider();

var userService = (UserService)provider.GetService(typeof(UserService));

services.AddRoleBasedAuthorizationWIthRules(userService.GetRolePolicies());

services.AddSwaggerGen(c =>
                {
                    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Testing", Version = "v1", Description = "some description" });

                    c.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
                    {
                        Type = SecuritySchemeType.OAuth2,
                        Flows = new OpenApiOAuthFlows
                        {
                            Implicit = new OpenApiOAuthFlow
                            {
                                AuthorizationUrl =
                                    new Uri(
                                        $"{AuthConfig.Authority}authorize?audience={AuthConfig.Audience}"),
                                Scopes = new Dictionary<string, string>()
                                {
                                    {"openid profile email", "Get all required info from Auth provider" }
                                }
                            }
                        }
                    });

                    c.OperationFilter<SecurityRequirementsOperationFilter>();
                })
                .AddSwaggerGenNewtonsoftSupport();

var app = builder.Build();

if (app.Environment.IsDevelopment()) 
{    
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BattlelineUtilities.AspNetCore.Auth.Example v1")
}

app.UseHttpsRedirection();
app.UseApiVersioning();
app.UseRouting();
app.UseAuthentication();
app.UseCustomEmailClaim("http://email.test.come/email")
app.UsePermissions(userService.GetPermissions());
app.UseAuthorization();
app.UseEndpoints(endpoints => endpoints.MapControllers());

app.Run();
```