using System.Security.Claims;

namespace BattlelineUtilities.AspNetCore.Auth;

public interface IRoleValidator
{
    bool UserHasRole(ClaimsIdentity user, string roleName);
}
