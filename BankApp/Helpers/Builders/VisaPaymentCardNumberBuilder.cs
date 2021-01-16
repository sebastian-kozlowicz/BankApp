using BankApp.Interfaces;
using System;

namespace BankApp.Helpers.Builders
{
    public class VisaPaymentCardNumberBuilder : IPaymentCardNumberBuilder
    {
        private static readonly int VisaPrefix = 4;

        public string GenerateCardNumber(int length)
        {
            return Guid.NewGuid().ToString().Substring(0, 15);
        }
    }
}
