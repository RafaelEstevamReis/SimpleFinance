namespace Simple.Finance.WebApi.Auth;

using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Simple.Finance.WebApi.Data;
using System;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

/// <summary>
/// Authenticates a request by the account Key sent on the <see cref="ApiKeyDefaults.HeaderName"/> header.
/// The Key is the whole credential: holding it grants full access to that account's database
/// </summary>
public class ApiKeyAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private readonly ManagementDb management;

    public ApiKeyAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                                       ILoggerFactory logger,
                                       UrlEncoder encoder,
                                       ManagementDb management)
        : base(options, logger, encoder)
    {
        this.management = management;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(ApiKeyDefaults.HeaderName, out var headerValues))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        var rawKey = headerValues.ToString();
        if (string.IsNullOrWhiteSpace(rawKey))
        {
            return Task.FromResult(AuthenticateResult.Fail($"'{ApiKeyDefaults.HeaderName}' is empty"));
        }
        if (!Guid.TryParse(rawKey, out var accountKey))
        {
            return Task.FromResult(AuthenticateResult.Fail($"'{ApiKeyDefaults.HeaderName}' is not a valid Key"));
        }

        var account = management.GetAccount(accountKey);
        if (account is null)
        {
            Logger.LogWarning("Rejected unknown Key from {RemoteIp}", Request.HttpContext.Connection.RemoteIpAddress);
            return Task.FromResult(AuthenticateResult.Fail("Unknown Key"));
        }
        if (!account.IsEnabled)
        {
            Logger.LogWarning("Rejected disabled account {AccountKey}", account.Key);
            return Task.FromResult(AuthenticateResult.Fail("Account is disabled"));
        }

        management.TouchLastAccess(account.Key);

        var identity = new ClaimsIdentity(
        [
            new Claim(ApiKeyDefaults.AccountKeyClaim, account.Key.ToString("D")),
            new Claim(ClaimTypes.Name, account.Name),
        ], Scheme.Name);

        var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name);
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
