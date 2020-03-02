using Binebase.Exchange.Common.Application.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Threading.Tasks;

namespace Binebase.Exchange.Common.Api
{
    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public CustomExceptionHandlerMiddleware(RequestDelegate next, ILogger logger)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch (exception)
            {
                case ValidationException validationException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(validationException.Failures);
                    break;
                case NotSupportedException _:
                    code = HttpStatusCode.BadRequest;
                    break;
                case NotFoundException _:
                    code = HttpStatusCode.NotFound;
                    break;
                case SecurityException _:
                    code = HttpStatusCode.Unauthorized;
                    break;
                case NotImplementedException _:
                    code = HttpStatusCode.NotImplemented;
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            if (string.IsNullOrWhiteSpace(result))
            {
                result = JsonConvert.SerializeObject(new { Error = new[] { exception.Message } });
            }

            _logger.LogError(exception, $"Unhandled exception is mapped to status code {context.Response.StatusCode}.");

            return context.Response.WriteAsync(result);
        }
    }

    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
            => builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
    }
}
