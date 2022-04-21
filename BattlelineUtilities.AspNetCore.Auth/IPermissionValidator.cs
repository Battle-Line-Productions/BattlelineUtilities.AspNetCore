using System.Security.Claims;

namespace BattlelineUtilities.AspNetCore.Auth;

public interface IPermissionValidator
{
    /// <summary>
    ///     Will check if user has permission. See example implementation
    /// </summary>
    /// <param name="user"></param>
    /// <param name="permission"></param>
    /// <returns></returns>
    bool UserHasPermission(ClaimsIdentity user, string permission);
}
