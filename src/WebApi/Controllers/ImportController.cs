namespace Simple.Finance.WebApi.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simple.Finance.Importers;
using Simple.Finance.Importers.MT940;
using Simple.Finance.Importers.OFX;
using Simple.Finance.WebApi.AccountManagement;
using Simple.Finance.WebApi.DTOs;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

/// <summary>
/// Statement import: reads a bank file and answers with the transactions it contains.
/// Nothing is written to the database and nothing is deduplicated — the parsed rows come
/// back in the very shape /api/transactions accepts, and what to keep is the client's call
/// </summary>
public class ImportController(ManagerCache managers) : AccountControllerBase(managers)
{
    /// <summary>
    /// Largest statement accepted. Statements are text and small; this only stops abuse
    /// </summary>
    private const long maxFileSize = 512 * 1024;

    /// <summary>
    /// Parses an OFX statement (bank or credit card, XML or the older SGML form).
    /// Every row comes back as Paid, with the posted date on both dates and the
    /// FitId appended to the description, which is the only handle the client has
    /// to recognise a row it already imported
    /// </summary>
    [HttpPost("ofx")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(maxFileSize)]
    [ProducesResponseType(typeof(TransactionRequest[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TransactionRequest[]> Ofx([FromForm] ImportRequest request)
    {
        var rejected = validate(request);
        if (rejected is not null) return rejected;

        var ofx = OfxFile.FromXML(readText(request.File!));
        if (ofx is null) return BadRequest("File is not an OFX statement");

        return TransactionImporter.FromOFX(ofx,
                                           request.WalletId,
                                           request.DefaultIncomeCategoryId,
                                           request.DefaultExpenseCategoryId)
                                  .Select(TransactionRequest.From)
                                  .ToArray();
    }

    /// <summary>
    /// Parses an MT940 statement. Every row comes back as Paid, with the entry date on
    /// both dates and the reference for the owner as the description; a 'D' mark makes
    /// the value negative
    /// </summary>
    [HttpPost("mt940")]
    [Consumes("multipart/form-data")]
    [RequestSizeLimit(maxFileSize)]
    [ProducesResponseType(typeof(TransactionRequest[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status501NotImplemented)]
    public ActionResult<TransactionRequest[]> Mt940([FromForm] ImportRequest request)
    {
        var rejected = validate(request);
        if (rejected is not null) return rejected;

        var statement = MT940Parser.FromLines(readLines(request.File!));

        return TransactionImporter.FromMT940(statement,
                                             request.WalletId,
                                             request.DefaultIncomeCategoryId,
                                             request.DefaultExpenseCategoryId)
                                  .Select(TransactionRequest.From)
                                  .ToArray();
    }

    /// <summary>
    /// The wallet and the categories are checked here because the import never reaches the
    /// Manager: without this the client would only learn the ids are wrong one POST later
    /// </summary>
    private ActionResult? validate(ImportRequest request)
    {
        if (request.File is null || request.File.Length == 0) return BadRequest("'File' is required");

        if (Manager.GetWallets().All(o => o.Id != request.WalletId))
        {
            return BadRequest($"Invalid Wallet Id: {request.WalletId}");
        }

        var categories = Manager.GetCategories().ToDictionary(o => o.Id);

        return checkCategory(categories, request.DefaultExpenseCategoryId, nameof(ImportRequest.DefaultExpenseCategoryId), isExpense: true)
            ?? checkCategory(categories, request.DefaultIncomeCategoryId, nameof(ImportRequest.DefaultIncomeCategoryId), isExpense: false);
    }

    /// <summary>
    /// The sign the bank gave the row is what picks between the two categories, so a category
    /// on the wrong side would land on exactly the rows whose sign the Manager then flips
    /// </summary>
    private ActionResult? checkCategory(Dictionary<long, Tables.Category> categories, long categoryId, string field, bool isExpense)
    {
        if (categoryId == 0) return null;
        if (!categories.TryGetValue(categoryId, out var category)) return BadRequest($"Invalid Category Id: {categoryId}");

        if (category.IsExpense != isExpense)
        {
            return BadRequest($"'{field}' must {(isExpense ? "" : "not ")}be 'IsExpense'");
        }

        return null;
    }

    private static string[] readLines(IFormFile file)
        => readText(file).Split(['\r', '\n'], StringSplitOptions.RemoveEmptyEntries);

    /// <summary>
    /// Reads the upload as text. Bank files are commonly Windows-1252, so bytes that are
    /// not valid UTF-8 are decoded as Latin1 instead of becoming replacement characters
    /// </summary>
    private static string readText(IFormFile file)
    {
        using var upload = file.OpenReadStream();
        using var buffer = new MemoryStream();
        upload.CopyTo(buffer);

        var bytes = buffer.ToArray();
        string text;
        try
        {
            text = new UTF8Encoding(false, throwOnInvalidBytes: true).GetString(bytes);
        }
        catch (ArgumentException) // DecoderFallbackException: not UTF-8
        {
            text = Encoding.Latin1.GetString(bytes);
        }

        // A BOM survives GetString and would corrupt the first tag or field
        return text.Length > 0 && text[0] == '\uFEFF' ? text.Substring(1) : text;
    }
}
