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

            if (NotFoundExceptions(e))
                return HttpStatusCode.NotFound;

            return HttpStatusCode.InternalServerError;
        }

        public static bool BadRequestExceptions(Exception e) => e is InvalidLoginException or RefreshTokenException;
        public static bool NotFoundExceptions(Exception e) => e is NotFoundException;
    }
}