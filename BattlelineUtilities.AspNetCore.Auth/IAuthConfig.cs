namespace BattlelineUtilities.AspNetCore.Auth;

public interface IAuthConfig
{
    /// <summary>
    ///     The Auth Authority Url
    /// </summary>
    public string Authority { get; set; }

    /// <summary>
    ///     Audience from your Auth config
    /// </summary>
    public string Audience { get; set; }
}
