using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace RedYellowGreen.Api.Infrastructure.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IProblemDetailsService _problemDetailsService;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IProblemDetailsService problemDetailsService)
    {
        _logger = logger;
        _problemDetailsService = problemDetailsService;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = exception switch
        {
            BadRequestException => StatusCodes.Status400BadRequest,
            NotFoundException => StatusCodes.Status404NotFound,
            _ => StatusCodes.Status500InternalServerError
        };

        var logLevel = httpContext.Response.StatusCode == StatusCodes.Status500InternalServerError
            ? LogLevel.Error
            : LogLevel.Warning;
        // log 'expected' exceptions as well, because even though they are not an error here,
        // they indicate a possible issue with the client
        _logger.Log(logLevel: logLevel, message: "Exception occured", exception: exception);

        return await _problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            Exception = exception,
            ProblemDetails = new ProblemDetails()
            {
                Title = "An error occurred",
                Type = exception.GetType().Name,
                Detail = exception.Message,
            }
        });
    }
}