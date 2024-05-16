using NitesInstitution.Utils.ExceptionHelper;
using NitesInstitution.Utils.ExceptionHelper.CustomExceptions;

namespace NitesInstitution
{
    public class ExceptionToStatusCodeMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionToStatusCodeMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                await HandleException(ex, httpContext);
            }
        }

        private async Task HandleException(Exception ex, HttpContext httpContext)
        {
            if (ex is NotAuthenticatedException)
            {
                httpContext.Response.StatusCode = 401;
                await httpContext.Response.WriteAsJsonAsync(new ExceptionModel
                {
                    Message = "Invalid token",
                    StatusCode = 401,
                    Success = false
                });
            }
            else if (ex is FileNotFoundException)
            {
                httpContext.Response.StatusCode = 204;
            }
            else
            {
                httpContext.Response.StatusCode = 500;
                await httpContext.Response.WriteAsJsonAsync(new ExceptionModel
                {
                    Message = "An error occurred",
                    StatusCode = 500,
                    Success = false
                });
            }

        }
    }
}