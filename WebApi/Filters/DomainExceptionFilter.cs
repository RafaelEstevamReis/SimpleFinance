namespace Simple.Finance.WebApi.Filters;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System;

/// <summary>
/// Turns the validation exceptions of the Manager into HTTP answers.
/// The library rejects invalid data by throwing, and those are client errors, not server faults
/// </summary>
public class DomainExceptionFilter : IExceptionFilter
{
    private readonly ILogger<DomainExceptionFilter> logger;

    public DomainExceptionFilter(ILogger<DomainExceptionFilter> logger)
    {
        this.logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        var (status, title) = context.Exception switch
        {
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid argument"),
            InvalidOperationException => (StatusCodes.Status400BadRequest, "Invalid operation"),
            NotImplementedException => (StatusCodes.Status501NotImplemented, "Not supported"),
            _ => (0, string.Empty),
        };
        if (status == 0) return; // Anything else is a real fault, let it become a 500

        logger.LogWarning("Rejected {Method} {Path}: {Message}", context.HttpContext.Request.Method, context.HttpContext.Request.Path, context.Exception.Message);

        context.Result = new ObjectResult(new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = context.Exception.Message,
        })
        { StatusCode = status };
        context.ExceptionHandled = true;
    }
}
