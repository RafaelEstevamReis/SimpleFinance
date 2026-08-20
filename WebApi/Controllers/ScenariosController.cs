namespace Simple.Finance.WebApi.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simple.Finance.WebApi.AccountManagement;
using Simple.Finance.WebApi.DTOs;
using System;
using System.Linq;

/// <summary>
/// Scenarios: named sets of hypothetical movements used to compare futures.
/// Nothing here reaches the real transactions, and nothing here is logged on the change log
/// </summary>
public class ScenariosController(ManagerCache managers) : AccountControllerBase(managers)
{
    /// <summary>
    /// Every scenario, active or not
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ScenarioResponse[]), StatusCodes.Status200OK)]
    public ActionResult<ScenarioResponse[]> GetAll()
        => Manager.GetScenarios().Select(ScenarioResponse.From).ToArray();

    /// <summary>
    /// A single scenario
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ScenarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ScenarioResponse> Get(long id)
    {
        var scenario = Manager.GetScenarioById(id);
        if (scenario is null) return NotFound();

        return ScenarioResponse.From(scenario);
    }

    /// <summary>
    /// Creates a scenario
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(ScenarioResponse), StatusCodes.Status201Created)]
    public ActionResult<ScenarioResponse> Create([FromBody] ScenarioRequest request)
    {
        var scenario = request.ToTable(0);
        Manager.CreateUpdateScenario(scenario);

        return CreatedAtAction(nameof(Get), new { id = scenario.Id }, ScenarioResponse.From(scenario));
    }

    /// <summary>
    /// Updates a scenario
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ScenarioResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ScenarioResponse> Update(long id, [FromBody] ScenarioRequest request)
    {
        if (Manager.GetScenarioById(id) is null) return NotFound();

        var scenario = request.ToTable(id);
        Manager.CreateUpdateScenario(scenario);

        return ScenarioResponse.From(scenario);
    }

    /// <summary>
    /// Deletes a scenario and all its items. Scenarios are drafts, not money records,
    /// so this is a real delete and it cannot be undone
    /// </summary>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(long id)
    {
        if (Manager.GetScenarioById(id) is null) return NotFound();

        Manager.DeleteScenario(id);
        return NoContent();
    }

    /// <summary>
    /// Turns many scenarios on or off at once.
    /// </summary>
    [HttpPut("active")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult SetActive([FromBody] ScenarioToggleRequest request)
    {
        Manager.SetScenarioActive(request.Ids, request.State);
        return NoContent();
    }

    /// <summary>
    /// Items of one scenario, enabled or not
    /// </summary>
    [HttpGet("{id:long}/items")]
    [ProducesResponseType(typeof(ScenarioItemResponse[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ScenarioItemResponse[]> GetItems(long id)
    {
        if (Manager.GetScenarioById(id) is null) return NotFound();

        return Manager.GetScenarioItems(id).Select(ScenarioItemResponse.From).ToArray();
    }

    /// <summary>
    /// A single item of a scenario
    /// </summary>
    [HttpGet("{id:long}/items/{itemId:long}")]
    [ProducesResponseType(typeof(ScenarioItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ScenarioItemResponse> GetItem(long id, long itemId)
    {
        var item = findItem(id, itemId);
        if (item is null) return NotFound();

        return ScenarioItemResponse.From(item);
    }

    /// <summary>
    /// Adds an item to a scenario. The sign of the value comes from the category,
    /// so positive values are fine
    /// </summary>
    [HttpPost("{id:long}/items")]
    [ProducesResponseType(typeof(ScenarioItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ScenarioItemResponse> CreateItem(long id, [FromBody] ScenarioItemRequest request)
    {
        if (Manager.GetScenarioById(id) is null) return NotFound();

        var item = request.ToTable(0, id);
        Manager.CreateUpdateScenarioItem(item);

        return CreatedAtAction(nameof(GetItem), new { id, itemId = item.Id }, ScenarioItemResponse.From(item));
    }

    /// <summary>
    /// Adds or replaces many items at once, on a single connection.
    /// Entries are applied in order, so an invalid one answers 400 and leaves the previous ones stored
    /// </summary>
    [HttpPost("{id:long}/items/bulk")]
    [ProducesResponseType(typeof(ScenarioItemResponse[]), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ScenarioItemResponse[]> CreateItemsBulk(long id, [FromBody] ScenarioItemBulkRequest[] request)
    {
        if (Manager.GetScenarioById(id) is null) return NotFound();

        var items = request.Select(o => o.ToTable(o.Id, id)).ToArray();
        Manager.CreateUpdateBulkScenarioItem(items);

        return items.Select(ScenarioItemResponse.From).ToArray();
    }

    /// <summary>
    /// Replaces an item of a scenario
    /// </summary>
    [HttpPut("{id:long}/items/{itemId:long}")]
    [ProducesResponseType(typeof(ScenarioItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<ScenarioItemResponse> UpdateItem(long id, long itemId, [FromBody] ScenarioItemRequest request)
    {
        if (findItem(id, itemId) is null) return NotFound();

        var item = request.ToTable(itemId, id);
        Manager.CreateUpdateScenarioItem(item);

        return ScenarioItemResponse.From(item);
    }

    /// <summary>
    /// Deletes one item of a scenario. Real delete, same as the scenario itself
    /// </summary>
    [HttpDelete("{id:long}/items/{itemId:long}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteItem(long id, long itemId)
    {
        if (findItem(id, itemId) is null) return NotFound();

        Manager.DeleteScenarioItem(itemId);
        return NoContent();
    }

    /// <summary>
    /// Enables or disables many items at once.
    /// </summary>
    [HttpPut("items/enabled")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult SetItemsEnabled([FromBody] ScenarioToggleRequest request)
    {
        Manager.SetScenarioItemEnabled(request.Ids, request.State);
        return NoContent();
    }

    /// <summary>
    /// Items of every wallet on a date window, oldest first.
    /// An item is active when its scenario is active and the item itself is enabled:
    /// 'isActive' true keeps only those, false only the others, omitted keeps all of them
    /// </summary>
    [HttpGet("projection")]
    [ProducesResponseType(typeof(ScenarioItemResponse[]), StatusCodes.Status200OK)]
    public ActionResult<ScenarioItemResponse[]> Project([FromQuery] DateTime start,
                                                        [FromQuery] DateTime end,
                                                        [FromQuery] bool? isActive)
        => Manager.ProjectScenariosItems(start, end, isActive).Select(ScenarioItemResponse.From).ToArray();

    /// <summary>
    /// Every active item of one wallet, oldest first, with no date window
    /// </summary>
    [HttpGet("projection/{walletId:long}")]
    [ProducesResponseType(typeof(ScenarioItemResponse[]), StatusCodes.Status200OK)]
    public ActionResult<ScenarioItemResponse[]> ProjectWallet(long walletId)
        => Manager.ProjectScenariosItemsFor(walletId).Select(ScenarioItemResponse.From).ToArray();

    private Tables.ScenarioItem? findItem(long scenarioId, long itemId)
    {
        var item = Manager.GetScenarioItemById(itemId);
        // An item of another scenario is not addressable through this route
        return item?.ScenarioId == scenarioId ? item : null;
    }
}
