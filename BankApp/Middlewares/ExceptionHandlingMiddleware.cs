using System;
using System.Threading.Tasks;
using BankApp.Helpers.Builders.ExceptionResponse;
using Microsoft.AspNetCore.Http;

namespace BankApp.Middlewares
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception e) when (StatusCodeFromExceptionBuilder.BadRequestExceptions(e))
            {
                context.Response.StatusCode = (int)StatusCodeFromExceptionBuilder.GetHttpStatusCodeFromException(e);
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(e.Message);
            }
        }
       
    }
}