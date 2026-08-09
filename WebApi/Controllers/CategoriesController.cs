namespace Simple.Finance.WebApi.Controllers;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Simple.Finance.WebApi.AccountManagement;
using Simple.Finance.WebApi.DTOs;
using System.Linq;

/// <summary>
/// Categories: what the money is for. IsExpense decides the sign of every transaction
/// </summary>
public class CategoriesController : AccountControllerBase
{
    public CategoriesController(ManagerCache managers) : base(managers) { }

    /// <summary>
    /// Every category, including the ones flagged as deleted
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(CategoryResponse[]), StatusCodes.Status200OK)]
    public ActionResult<CategoryResponse[]> GetAll()
        => Manager.GetCategories().Select(CategoryResponse.From).ToArray();

    /// <summary>
    /// A single category
    /// </summary>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CategoryResponse> Get(long id)
    {
        var category = find(id);
        if (category is null) return NotFound();

        return CategoryResponse.From(category);
    }

    /// <summary>
    /// Creates a category
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status201Created)]
    public ActionResult<CategoryResponse> Create([FromBody] CategoryRequest request)
    {
        var category = request.ToTable(0);
        Manager.CreateUpdateCategory(category);

        return CreatedAtAction(nameof(Get), new { id = category.Id }, CategoryResponse.From(category));
    }

    /// <summary>
    /// Updates a category. IsExpense cannot change after creation
    /// </summary>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(CategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<CategoryResponse> Update(long id, [FromBody] CategoryRequest request)
    {
        if (find(id) is null) return NotFound();

        var category = request.ToTable(id);
        Manager.CreateUpdateCategory(category);

        return CategoryResponse.From(category);
    }

    private Tables.Category? find(long id) => Manager.GetCategories().FirstOrDefault(o => o.Id == id);
}
