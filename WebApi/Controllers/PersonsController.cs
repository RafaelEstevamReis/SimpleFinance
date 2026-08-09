namespace Simple.Finance.WebApi.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simple.Finance.WebApi.AccountManagement;
using Simple.Finance.WebApi.DTOs;
using System.Linq;

/// <summary>
/// Counterparties: who the money came from or went to
/// </summary>
public class PersonsController : AccountControllerBase
{
    public PersonsController(ManagerCache managers) : base(managers) { }

    /// <summary>
    /// Every counterparty, including the ones flagged as deleted
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PersonResponse[]), StatusCodes.Status200OK)]
    public ActionResult<PersonResponse[]> GetAll()
        => Manager.GetAllPersons().Select(PersonResponse.From).ToArray();

    /// <summary>
    /// A single counterparty
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PersonResponse> Get(long id)
    {
        var person = find(id);
        if (person is null) return NotFound();

        return PersonResponse.From(person);
    }

    /// <summary>
    /// Creates a counterparty
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status201Created)]
    public ActionResult<PersonResponse> Create([FromBody] PersonRequest request)
    {
        var person = request.ToTable(0);
        Manager.CreateUpdatePerson(person);

        return CreatedAtAction(nameof(Get), new { id = person.Id }, PersonResponse.From(person));
    }

    /// <summary>
    /// Updates a counterparty
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(PersonResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PersonResponse> Update(long id, [FromBody] PersonRequest request)
    {
        if (find(id) is null) return NotFound();

        var person = request.ToTable(id);
        Manager.CreateUpdatePerson(person);

        return PersonResponse.From(person);
    }

    private Tables.Person? find(long id) => Manager.GetAllPersons().FirstOrDefault(o => o.Id == id);
}
