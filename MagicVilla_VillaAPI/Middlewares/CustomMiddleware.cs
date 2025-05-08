using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;


namespace MagicVilla_VillaAPI.Middlewares
{
    public class CustomMiddleware
    {
        private readonly RequestDelegate _next;

        public CustomMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
           
            var stopwatch = Stopwatch.StartNew();
            try
            {
                
                Console.WriteLine($"[{DateTime.UtcNow}] Incoming request: {context.Request.Method} {context.Request.Path}");

                await _next(context); 

                
                Console.WriteLine($"[{DateTime.UtcNow}] Response status: {context.Response.StatusCode}");
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                Console.WriteLine($"[{DateTime.UtcNow}] ERROR: {ex.Message}");

                await HandleExceptionAsync(context, ex);
            }
            finally
            {
                stopwatch.Stop();
                Console.WriteLine($"[{DateTime.UtcNow}] Request completed in {stopwatch.ElapsedMilliseconds}ms");
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = exception switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                _ => (int)HttpStatusCode.InternalServerError
            };

            var response = new
            {
                statusCode = context.Response.StatusCode,
                isSuccess = false,
                errorMessages = new[] { exception.Message }
            };

            return context.Response.WriteAsync(JsonConvert.SerializeObject(response));
        }
    }
}
   