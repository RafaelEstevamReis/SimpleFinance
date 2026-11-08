namespace Simple.Finance.WebApi.Auth;

using System;
using System.Security.Claims;

public static class ClaimsPrincipalExtensions
{
    /// <summary>
    /// Account Key of the authenticated request.
    /// Only valid where authentication is required, throws otherwise
    /// </summary>
    public static Guid GetAccountKey(this ClaimsPrincipal user)
    {
        var claim = user.FindFirst(ApiKeyDefaults.AccountKeyClaim)
            ?? throw new InvalidOperationException("Request is not authenticated");

        return Guid.Parse(claim.Value);
    }
}
