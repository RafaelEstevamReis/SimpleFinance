namespace Simple.Finance.WebApi.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Simple.Finance.WebApi.DTOs;
using System;

/// <summary>
/// Public endpoints, no Key required
/// </summary>
[ApiController]
[AllowAnonymous]
[Route("api/[controller]")]
[Produces("application/json")]
public class HelloController : ControllerBase
{
    private readonly ILogger<HelloController> logger;

    public HelloController(ILogger<HelloController> logger)
    {
        this.logger = logger;
    }

    /// <summary>
    /// Liveness check, also reports the running version
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(HelloResponse), StatusCodes.Status200OK)]
    public ActionResult<HelloResponse> Get()
    {
        logger.LogInformation("HelloWorld requested");

        return new HelloResponse
        {
            Message = "Hello World",
            Service = ApiInfo.Title,
            Version = ApiInfo.Version,
            UtcNow = DateTime.UtcNow,
        };
    }
}
