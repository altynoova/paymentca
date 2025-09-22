using Microsoft.AspNetCore.Diagnostics;

namespace WebApi
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(
            HttpContext http, Exception ex, CancellationToken ct)
        {
            var status = ex switch
            {
                UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
                InvalidOperationException => StatusCodes.Status400BadRequest,
                _ => StatusCodes.Status500InternalServerError
            };

            http.Response.StatusCode = status;
            await http.Response.WriteAsJsonAsync(new
            {
                error = ex.Message,
                status
            }, ct);

            return true;
        }
    }
}
