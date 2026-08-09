namespace Simple.Finance.WebApi.Controllers;

using Microsoft.AspNetCore.Mvc;
using Simple.Finance.WebApi.AccountManagement;
using Simple.Finance.WebApi.Auth;
using System;

/// <summary>
/// Base for every controller that works on the finance data of the authenticated account.
/// Resolves the Key of the request into that account's <see cref="Manager"/>
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public abstract class AccountControllerBase : ControllerBase
{
    private readonly ManagerCache managers;
    private Manager? manager;

    protected AccountControllerBase(ManagerCache managers)
    {
        this.managers = managers;
    }

    /// <summary>
    /// Key of the authenticated account
    /// </summary>
    protected Guid AccountKey => User.GetAccountKey();

    /// <summary>
    /// Manager of the authenticated account, initialized on first touch
    /// </summary>
    protected Manager Manager => manager ??= managers.GetFor(AccountKey);
}
