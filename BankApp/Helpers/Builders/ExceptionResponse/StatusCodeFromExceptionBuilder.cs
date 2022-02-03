using System;
using System.Net;
using BankApp.Exceptions;

namespace BankApp.Helpers.Builders.ExceptionResponse
{
    public static class StatusCodeFromExceptionBuilder
    {
        public static HttpStatusCode GetHttpStatusCodeFromException(Exception e)
        {
            if (BadRequestExceptions(e))
                return HttpStatusCode.BadRequest;

            return HttpStatusCode.InternalServerError;
        }

        public static bool BadRequestExceptions(Exception e) => e is InvalidLoginException or RefreshTokenException;
    }
}