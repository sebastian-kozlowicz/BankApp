using System;
using System.Globalization;

namespace BankApp.Exceptions
{
    public class RefreshTokenException : Exception
    {
        public RefreshTokenException()
        {
        }

        public RefreshTokenException(string message) : base(message)
        {
        }

        public RefreshTokenException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}