namespace Simple.Finance.WebApi.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simple.Finance.WebApi.AccountManagement;
using Simple.Finance.WebApi.DTOs;

/// <summary>
/// Transfers between wallets. They are a linked pair of transactions and must
/// never be created or edited through /api/transactions
/// </summary>
public class TransfersController(ManagerCache managers) : AccountControllerBase(managers)
{
    /// <summary>
    /// Both legs of a transfer, found by the id of either one of them
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(TransferResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TransferResponse> Get(long id)
    {
        var tx = Manager.GetTransactionById(id);
        if (tx is null || tx.Type != Tables.Transac.TransactionType.WalletTransfer) return NotFound();

        return pairOf(tx);
    }

    /// <summary>
    /// Moves money between two wallets, writing both legs at once.
    /// The source category must be an expense and the destination category must not be
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TransferResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TransferResponse> Create([FromBody] CreateTransferRequest request)
    {
        var (payId, receiveId) = Manager.CreateWalletTransfer(request.SourceWalletId,
                                                              request.SourceCategoryId,
                                                              request.DestinationWalletId,
                                                              request.DestinationCategoryId,
                                                              request.Description,
                                                              request.Value,
                                                              request.DueDate,
                                                              request.PaymentDate,
                                                              request.Paid,
                                                              request.PaymentDetails);

        // Read back instead of echoing the request: the pair only gets its
        // Type and TypeOtherId after the Manager cross-links both legs
        var response = pairOf(Manager.GetTransactionById(payId)!);

        return CreatedAtAction(nameof(Get), new { id = payId }, response);
    }

    /// <summary>
    /// Updates both legs of a transfer at once, found by the id of either one,
    /// and answers with the pair as it was persisted
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(TransferResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TransferResponse> Update(long id, [FromBody] UpdateTransferRequest request)
    {
        var tx = Manager.GetTransactionById(id);
        if (tx is null || tx.Type != Tables.Transac.TransactionType.WalletTransfer) return NotFound();

        Manager.UpdateWalletTransfer(id,
                                     request.DueValue,
                                     request.PaidValue,
                                     request.DueDate,
                                     request.PaymentDate,
                                     request.Description,
                                     request.PaymentDetails,
                                     request.Status);

        return pairOf(Manager.GetTransactionById(id)!);
    }

    private TransferResponse pairOf(Tables.Transac oneLeg)
    {
        var (source, destination) = Manager.GetTransferPair(oneLeg);

        return new TransferResponse
        {
            Source = TransactionResponse.From(source),
            Destination = TransactionResponse.From(destination),
        };
    }
}
