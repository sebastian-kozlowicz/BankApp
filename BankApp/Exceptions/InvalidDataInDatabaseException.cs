using System;
using System.Globalization;

namespace BankApp.Exceptions
{
    public class InvalidDataInDatabaseException : Exception
    {
        public InvalidDataInDatabaseException()
        {
        }

        public InvalidDataInDatabaseException(string message) : base(message)
        {
        }

        public InvalidDataInDatabaseException(string message, params object[] args)
            : base(string.Format(CultureInfo.CurrentCulture, message, args))
        {
        }
    }
}