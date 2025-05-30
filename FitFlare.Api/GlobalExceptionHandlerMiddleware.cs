using FitFlare.Application.Helpers.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace FitFlare.Api;

public class GlobalExceptionHandlerMiddleware : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            UnauthorizedAccessException ex => new ProblemDetails
            {
                Title = "Unauthorized",
                Detail = ex.Message,
                Status = StatusCodes.Status401Unauthorized
            },
            UserAlreadyExistsException ex => new ProblemDetails
            {
                Title = "User already exists",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            },
            InternalServerErrorException ex => new ProblemDetails
            {
                Title = "Internal server error",
                Detail = ex.Message,
                Status = StatusCodes.Status500InternalServerError
            },
            UserNotFoundException ex => new ProblemDetails
            {
                Title = "User not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            },
            BadRequestException ex => new ProblemDetails
            {
                Title = "Bad request",
                Detail = ex.Message,
                Status = StatusCodes.Status400BadRequest
            },
            NotFoundException ex => new ProblemDetails
            {
                Title = "Not found",
                Detail = ex.Message,
                Status = StatusCodes.Status404NotFound
            },
            _ => new ProblemDetails
            {
                Title = "Server Error",
                Detail = exception.Message,
                Status = StatusCodes.Status500InternalServerError
            }
        };

        httpContext.Response.StatusCode = problemDetails.Status ?? 500;
        httpContext.Response.ContentType = "application/json";

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        return true;
    }
}