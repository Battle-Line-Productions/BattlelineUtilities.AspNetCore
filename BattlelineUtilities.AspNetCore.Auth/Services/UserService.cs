using System.Security.Claims;

namespace BattlelineUtilities.AspNetCore.Auth.Services;

public class UserService : IRoleValidator, IPermissionValidator
{
    /// <summary>
    ///     Returns false if user does not match specific role and true if they do.
    /// </summary>
    /// <param name="user">The user claims to check against</param>
    /// <param name="permission">the permission you wish to check against</param>
    public virtual bool UserHasPermission(ClaimsIdentity user, string permission)
    {
        return permission == "User:Can-Read";
    }

    /// <summary>
    ///     Returns false if user does not match specific permission and true if they do.
    /// </summary>
    /// <param name="user">The user claims to check against</param>
    /// <param name="roleName">the role you wish to check against</param>
    public virtual bool UserHasRole(ClaimsIdentity user, string roleName)
    {
        return roleName == "User";
    }

    /// <summary>
    ///     This is an example of how to return a list of permissions where User is the data set and Can-Edit or Can-Read is
    ///     the permissions for that data set.
    ///     Use this to control a very granular set of permissions
    /// </summary>
    public virtual List<string> GetPermissions()
    {
        return new List<string>
        {
            "User:Can-Edit",
            "User:Can-Read"
        };
    }

    /// <summary>
    ///     This is an example of how to return a list of roles.
    ///     Use this to control a grouping of roles for your application
    /// </summary>
    public virtual List<string> GetRoles()
    {
        return new List<string>
        {
            "Administrator",
            "User"
        };
    }

    /// <summary>
    ///     Use this to get a list of roles and the policies in those roles.
    ///     Example: Role EveryUser will allow anyone that has a claim containing the roles Administrator or User.
    /// </summary>
    public virtual Dictionary<string, List<string>> GetRolePolicies()
    {
        return new Dictionary<string, List<string>>
        {
            {"EveryRole", new List<string> {"Administrator", "User"}},
            {"Administrator", new List<string> {"Administrator"}},
            {"User", new List<string> {"User"}}
        };
    }
}
