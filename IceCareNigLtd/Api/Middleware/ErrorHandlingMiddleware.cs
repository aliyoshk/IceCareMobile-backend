using System;
using System.Net;
using System.Text.Json;
using IceCareNigLtd.Api.Middleware;
using IceCareNigLtd.Api.Models.Network;

namespace IceCareNigLtd.Api.Request
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
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

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode statusCode;
            ErrorResponse response;

            switch (exception)
            {
                case ArgumentException _:
                    statusCode = HttpStatusCode.BadRequest;
                    response = new ErrorResponse
                    {
                        Success = false,
                        Message = "Bad request.",
                        Errors = new List<string> { exception.Message }
                    };
                    break;

                case CustomerNotFoundException ex:
                    statusCode = HttpStatusCode.NotFound;
                    response = new ErrorResponse
                    {
                        Success = false,
                        Message = ex.Message,
                        Errors = new List<string> { $"{ex.EntityName} not found." }
                    };
                    break;

                // Add more custom exceptions here

                default:
                    statusCode = HttpStatusCode.InternalServerError;
                    response = new ErrorResponse
                    {
                        Success = false,
                        Message = "An unexpected error occurred.",
                        Errors = new List<string> { exception.Message }
                    };
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)statusCode;

            var result = JsonSerializer.Serialize(response);
            return context.Response.WriteAsync(result);
        }
    }
}


