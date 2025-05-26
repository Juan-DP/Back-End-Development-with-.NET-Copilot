using Microsoft.AspNetCore.Http;
using Serilog;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace UserManagementAPI.Middleware
{
    public class RequestResponseLoggingMiddleware
    {
        private readonly RequestDelegate _next;

        public RequestResponseLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Log the HTTP Request
            var request = await FormatRequest(context.Request);
            Log.Information("HTTP Request: {Request}", request);

            // Capture the original response body stream
            var originalResponseBodyStream = context.Response.Body;

            using (var responseBody = new MemoryStream())
            {
                context.Response.Body = responseBody;

                // Continue down the middleware pipeline
                await _next(context);

                // Log the HTTP Response
                var response = await FormatResponse(context.Response);
                Log.Information("HTTP Response: {Response}", response);

                // Copy the response back to the original stream
                await responseBody.CopyToAsync(originalResponseBodyStream);
            }
        }

        private async Task<string> FormatRequest(HttpRequest request)
        {
            request.EnableBuffering();

            var body = string.Empty;
            if (request.ContentLength > 0)
            {
                using (var reader = new StreamReader(
                    request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    body = await reader.ReadToEndAsync();
                    request.Body.Position = 0;
                }
            }

            return $"{request.Method} {request.Path} {request.QueryString} {body}";
        }

        private async Task<string> FormatResponse(HttpResponse response)
        {
            response.Body.Seek(0, SeekOrigin.Begin);
            var text = await new StreamReader(response.Body).ReadToEndAsync();
            response.Body.Seek(0, SeekOrigin.Begin);
            return $"{response.StatusCode}: {text}";
        }
    }
}