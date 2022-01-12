using System.Net;
using System.Text.Json;
using Application.Core;

namespace API.Middleware
{
    public class ExceptionMiddleware
    {
        // Made private fields for the exception parameters 
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;
        // The constructor for the class
        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _env = env;
            _logger = logger;
            _next = next;
        }
        // The method for calling out server errors
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Let data flow to the next point if nothing went wrong
                await _next(context);
            }
            catch (Exception ex)
            {
                // If someting went wrong on server end
                // Grab the message, put it in a json fromat with a 500 error code
                _logger.LogError(ex, ex.Message);
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                // If the app is in dev environment send full message and stack trace otherwise send generic "Server Error" message
                var response = _env.IsDevelopment()
                    ? new AppException(context.Response.StatusCode, ex.Message, ex.StackTrace?.ToString())
                    : new AppException(context.Response.StatusCode, "Server Error");
                // Formatting options for camelCase
                var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
                // Setting up json response
                var json = JsonSerializer.Serialize(response, options);
                // Sending json response
                await context.Response.WriteAsync(json);
            }
        }
    }
}