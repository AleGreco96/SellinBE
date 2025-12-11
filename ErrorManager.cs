using System.Net;

namespace SellinBE
{
    public class ErrorManager
    {
        private readonly ILogger<ErrorManager> _logger;
        private readonly RequestDelegate _next;
    
        public ErrorManager(ILogger<ErrorManager> logger, RequestDelegate next)
        {
            _logger = logger;
            _next = next;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                var errorId = Guid.NewGuid();

                _logger.LogError($"[{errorId}] Unhandled exception: {ex.Message}");

                httpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                httpContext.Response.ContentType = "application/json";

                var error = new
                {
                    Id = errorId,
                    ErrorMessage = "Something went wrong."
                };

                await httpContext.Response.WriteAsJsonAsync(error);

            }
        }

    }
}
