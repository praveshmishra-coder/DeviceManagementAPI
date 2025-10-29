using System.Net;
using System.Text;
using System.Text.Json;

namespace DeviceManagementAPI.Middlewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // 🟢 Log basic request info
                _logger.LogInformation("➡️ {Method} {Path}", context.Request.Method, context.Request.Path);

                // 🟡 Log request body for POST/PUT/PATCH
                if (context.Request.Method is "POST" or "PUT" or "PATCH")
                {
                    context.Request.EnableBuffering(); // allows re-reading the body
                    using var reader = new StreamReader(context.Request.Body, Encoding.UTF8, leaveOpen: true);
                    var body = await reader.ReadToEndAsync();
                    context.Request.Body.Position = 0;

                    if (!string.IsNullOrWhiteSpace(body))
                        _logger.LogInformation("📦 Request Body: {Body}", body);
                    else
                        _logger.LogInformation("📦 Request Body: (empty)");
                }

                await _next(context); // continue the pipeline

                // 🟢 Log response status
                _logger.LogInformation("⬅️ Response Status: {StatusCode}", context.Response.StatusCode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "❌ Unhandled exception occurred while processing the request.");

                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";

                var response = new
                {
                    success = false,
                    error = new
                    {
                        message = "An unexpected error occurred. Please try again later.",
                        detail = ex.Message // ⚠️ Remove in production for security
                    }
                };

                var jsonResponse = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                await context.Response.WriteAsync(jsonResponse);
            }
        }
    }

    // ✅ Extension method for cleaner Program.cs registration
    public static class ErrorHandlingMiddlewareExtensions
    {
        public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ErrorHandlingMiddleware>();
        }
    }
}
