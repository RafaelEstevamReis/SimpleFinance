namespace Simple.Finance.WebApi.DTOs;

using Microsoft.AspNetCore.Http;

/// <summary>
/// A statement file to be parsed into transactions. Nothing is stored by the import,
/// the answer is a list of candidates the client decides what to do with
/// </summary>
public record ImportRequest
{
    /// <summary>
    /// The statement file itself
    /// </summary>
    public IFormFile? File { get; set; }

    /// <summary>
    /// Wallet the parsed transactions belong to. It must exist
    /// </summary>
    public long WalletId { get; set; }

    /// <summary>
    /// Category for the rows that came in positive, 0 for none. It must not be an expense
    /// </summary>
    public long DefaultIncomeCategoryId { get; set; }

    /// <summary>
    /// Category for the rows that came in negative, 0 for none. It must be an expense
    /// </summary>
    public long DefaultExpenseCategoryId { get; set; }
}
