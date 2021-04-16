using System;
using System.Net;
using System.Threading.Tasks;
using BankApp.Exceptions;
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
            catch (Exception e) when (ValidationExceptions(e))
            {
                context.Response.StatusCode = (int) HttpStatusCode.BadRequest;
                context.Response.ContentType = "text/plain";
                await context.Response.WriteAsync(e.Message);
            }
        }

        private static bool ValidationExceptions(Exception e)
        {
            return e is InvalidLoginException || e is RefreshTokenException;
        }
    }
}