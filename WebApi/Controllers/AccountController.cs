namespace Simple.Finance.WebApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Simple.Finance.WebApi.Auth;
using Simple.Finance.WebApi.Data;
using Simple.Finance.WebApi.Models;

/// <summary>
/// Accounts. Creating one is creating a new Key, and the Key is the only credential
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AccountController : ControllerBase
{
    private readonly ManagementDb management;
    private readonly ILogger<AccountController> logger;

    public AccountController(ManagementDb management, ILogger<AccountController> logger)
    {
        this.management = management;
        this.logger = logger;
    }

    /// <summary>
    /// Creates a new account and returns its Key.
    /// The Key is returned only by this call and cannot be recovered later
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(CreateAccountResponse), StatusCodes.Status201Created)]
    public ActionResult<CreateAccountResponse> Create([FromBody] CreateAccountRequest request)
    {
        var account = management.CreateAccount(request.Name?.Trim() ?? string.Empty);
        logger.LogInformation("Created account {AccountKey}", account.Key);

        return CreatedAtAction(nameof(Get), new CreateAccountResponse
        {
            Key = account.Key,
            Name = account.Name,
            Created = account.Created,
        });
    }

    /// <summary>
    /// The account behind the Key used on this request
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(AccountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<AccountResponse> Get()
    {
        var key = User.GetAccountKey();
        var account = management.GetAccount(key);
        if (account is null) return Unauthorized();

        return new AccountResponse
        {
            Key = account.Key,
            Name = account.Name,
            Created = account.Created,
            LastAccess = account.LastAccess,
        };
    }
}
