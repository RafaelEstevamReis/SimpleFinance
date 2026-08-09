namespace Simple.Finance.WebApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Simple.Finance.WebApi.Auth;
using Simple.Finance.WebApi.AccountManagement;
using System.IO;
using System.Linq;
using Simple.Finance.WebApi.DTOs;

/// <summary>
/// Accounts. Creating one is creating a new Key, and the Key is the only credential
/// </summary>
public class AccountController : AccountControllerBase
{
    private readonly ManagementDb management;
    private readonly ILogger<AccountController> logger;

    public AccountController(ManagementDb management, ManagerCache managers, ILogger<AccountController> logger)
        : base(managers)
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
        var key = AccountKey;
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

    /// <summary>
    /// Finance database of this account: file, size and last backup.
    /// Touching it is what creates and initializes the database on first use
    /// </summary>
    [HttpGet("database")]
    [ProducesResponseType(typeof(AccountDatabaseResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public ActionResult<AccountDatabaseResponse> GetDatabase()
    {
        var walletCount = Manager.GetWallets().Count(); // Forces the Manager to exist and the schema to be ready

        var file = new FileInfo(AppPaths.UserDbFile(AccountKey));
        var backups = new DirectoryInfo(AppPaths.UserBackupFolder(AccountKey));
        var lastBackup = backups.Exists
            ? backups.GetFiles("*.gz").OrderByDescending(o => o.LastWriteTimeUtc).FirstOrDefault()
            : null;

        return new AccountDatabaseResponse
        {
            FileName = file.Name,
            SizeBytes = file.Exists ? file.Length : 0,
            WalletCount = walletCount,
            LastBackup = lastBackup?.Name,
            LastBackupUtc = lastBackup?.LastWriteTimeUtc,
        };
    }

    /// <summary>
    /// Preferences of this account. They live on the management database,
    /// not on the finance one
    /// </summary>
    [HttpGet("preferences")]
    [ProducesResponseType(typeof(PreferenceResponse[]), StatusCodes.Status200OK)]
    public ActionResult<PreferenceResponse[]> GetPreferences()
        => management.GetPreferences(AccountKey).Select(PreferenceResponse.From).ToArray();

    /// <summary>
    /// Creates or replaces a single preference
    /// </summary>
    [HttpPut("preferences/{name}")]
    [ProducesResponseType(typeof(PreferenceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<PreferenceResponse> SetPreference(string name, [FromBody] PreferenceRequest request)
    {
        if (string.IsNullOrWhiteSpace(name)) return BadRequest("Preference name is required");

        var value = request.Value ?? string.Empty;
        management.SetPreference(AccountKey, name, value);

        return new PreferenceResponse { Name = name, Value = value };
    }
}
