namespace Simple.Finance.WebApi.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simple.Finance.WebApi.AccountManagement;
using Simple.Finance.WebApi.DTOs;
using System;
using System.Collections.Generic;

/// <summary>
/// Audit trail: every write the Manager made, field by field.
/// It is written automatically, there is no way to add or remove entries
/// </summary>
public class ChangeLogController(ManagerCache managers) : AccountControllerBase(managers)
{
    /// <summary>
    /// Every change event in a period, oldest first
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ChangeLogEntryResponse[]), StatusCodes.Status200OK)]
    public ActionResult<ChangeLogEntryResponse[]> Search([FromQuery] DateTime start,
                                                         [FromQuery] DateTime end,
                                                         [FromQuery] long? externalId)
    {
        var rows = externalId is null
            ? Manager.GetLogs(start, end)
            : Manager.GetLogs(start, end, externalId.Value);

        return ChangeLogEntryResponse.FromRows(rows);
    }

    /// <summary>
    /// Full history of a single record, oldest first.
    /// An unknown id simply has no history, it is not an error
    /// </summary>
    [HttpGet("{table}/{id:long}")]
    [ProducesResponseType(typeof(ChangeLogEntryResponse[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<ChangeLogEntryResponse[]> GetHistory(ChangeLogTable table, long id)
    {
        IEnumerable<Tables.TableLogRegistry> rows = table switch
        {
            ChangeLogTable.Wallet => Manager.GetLogs<Tables.Wallet>(id),
            ChangeLogTable.Category => Manager.GetLogs<Tables.Category>(id),
            ChangeLogTable.Person => Manager.GetLogs<Tables.Person>(id),
            ChangeLogTable.Transaction => Manager.GetLogs<Tables.Transac>(id),
            _ => throw new InvalidOperationException($"Unknown table '{table}'"),
        };

        return ChangeLogEntryResponse.FromRows(rows);
    }
}
