using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace MagicVilla_VillaAPI.Middlewares
{
    public class ExceptionMiddleware
    {
       private readonly RequestDelegate _next;
       private readonly ILogger<ExceptionMiddleware> _logger;

       public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
      {
        _next = next;
        _logger = logger;
      }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context); 
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception Occurred: {ex.Message}"); 
            await HandleExceptionAsync(context, ex); 
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json"; 
        if (exception is ArgumentException)  
        {
           context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
        }
       else
        {
          context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
        }

        var errorResponse = new
        {
            StatusCode = context.Response.StatusCode,
            Message = "An unexpected error occurred. Please try again later.",
            Details = exception.Message 
        };

        return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}
}
