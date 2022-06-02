using System;
using System.Globalization;

namespace BankApp.Exceptions
{
    public class InvalidInputDataException : Exception
    {
        public InvalidInputDataException()
        {
        }

        public InvalidInputDataException(string message) : base(message)
        {
        }

        public InvalidInputDataException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}