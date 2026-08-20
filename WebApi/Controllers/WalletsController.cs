namespace Simple.Finance.WebApi.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simple.Finance.WebApi.AccountManagement;
using Simple.Finance.WebApi.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Wallets: where the money is. Every balance is in the wallet's own currency
/// </summary>
public class WalletsController(ManagerCache managers) : AccountControllerBase(managers)
{
    /// <summary>
    /// Every wallet, including the ones flagged as deleted
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(WalletResponse[]), StatusCodes.Status200OK)]
    public ActionResult<WalletResponse[]> GetAll()
        => Manager.GetWallets().Select(WalletResponse.From).ToArray();

    /// <summary>
    /// A single wallet
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(WalletResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<WalletResponse> Get(long id)
    {
        var wallet = find(id);
        if (wallet is null) return NotFound();

        return WalletResponse.From(wallet);
    }

    /// <summary>
    /// Creates a wallet
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(WalletResponse), StatusCodes.Status201Created)]
    public ActionResult<WalletResponse> Create([FromBody] WalletRequest request)
    {
        var wallet = request.ToTable(0);
        Manager.CreateUpdateWallet(wallet);

        return CreatedAtAction(nameof(Get), new { id = wallet.Id }, WalletResponse.From(wallet));
    }

    /// <summary>
    /// Updates a wallet
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(WalletResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<WalletResponse> Update(long id, [FromBody] WalletRequest request)
    {
        if (find(id) is null) return NotFound();

        var wallet = request.ToTable(id);
        Manager.CreateUpdateWallet(wallet);

        return WalletResponse.From(wallet);
    }

    /// <summary>
    /// Balance of every wallet. Without a date it is the settled balance right now;
    /// with a future date it also adds what falls due until then
    /// </summary>
    [HttpGet("balances")]
    [ProducesResponseType(typeof(WalletBalanceResponse[]), StatusCodes.Status200OK)]
    public ActionResult<WalletBalanceResponse[]> GetBalances([FromQuery] DateTime? atDate)
    {
        var balances = (atDate is null ? Manager.GetWalletsBalance() : Manager.GetWalletsBalance(atDate.Value))
            .ToDictionary(o => o.WalletId, o => o.Balance);

        // Wallets without transactions are absent from the query, they are still worth reporting as zero
        return Manager.GetWallets()
                      .Select(wallet => new WalletBalanceResponse
                      {
                          WalletId = wallet.Id,
                          Name = wallet.Name,
                          BaseCurrency = wallet.BaseCurrency,
                          Balance = balances.GetValueOrDefault(wallet.Id, 0m),
                      })
                      .ToArray();
    }

    /// <summary>
    /// Settled balance of a single wallet: paid transactions up to now
    /// </summary>
    [HttpGet("{id:long}/balance")]
    [ProducesResponseType(typeof(WalletBalanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<WalletBalanceResponse> GetBalance(long id)
    {
        var wallet = find(id);
        if (wallet is null) return NotFound();

        return new WalletBalanceResponse
        {
            WalletId = wallet.Id,
            Name = wallet.Name,
            BaseCurrency = wallet.BaseCurrency,
            Balance = Manager.GetWalletBalance(id),
        };
    }

    private Tables.Wallet? find(long id) => Manager.GetWallets().FirstOrDefault(o => o.Id == id);
}
