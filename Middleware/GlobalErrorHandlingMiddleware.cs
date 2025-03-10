using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BookManagementSystem.Middleware
{
    public class GlobalErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalErrorHandlingMiddleware> _logger;

        public GlobalErrorHandlingMiddleware(RequestDelegate next, ILogger<GlobalErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next(httpContext);
            }
            catch (Exception ex)
            {
                // Log the error
                _logger.LogError($"An unexpected error occurred: {ex.Message}");

                // Set the response code to 500 (Internal Server Error)
                httpContext.Response.StatusCode = 500; // Internal Server Error

                // Optionally, you can return a generic message to the client
                await httpContext.Response.WriteAsync("An unexpected error occurred.");
            }
        }
    }
}
