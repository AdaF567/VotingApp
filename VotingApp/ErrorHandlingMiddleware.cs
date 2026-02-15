using Voting.Services.Consts;
using Voting.Services.Exceptions;

namespace Voting.Web.API
{
    public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogError(ex, "Not found.");

                context.Response.StatusCode = StatusCodes.Status404NotFound;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    error = ex.Message,
                    code = ErrorKeys.NOT_FOUND,
                });
            }
            catch (VotingInvalidOperationException ex)
            {
                _logger.LogError(ex, "Unexpected error occurred.");

                context.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    error = ex.Message,
                    code = ex.Message,
                });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Unexpected error occurred.");

                context.Response.StatusCode = StatusCodes.Status409Conflict;
                context.Response.ContentType = "application/json";

                await context.Response.WriteAsJsonAsync(new
                {
                    error = "Unexpected error occurred.",
                    code = ErrorKeys.UNEXPECTED_ERROR,
                });
            }
        }
    }
}
