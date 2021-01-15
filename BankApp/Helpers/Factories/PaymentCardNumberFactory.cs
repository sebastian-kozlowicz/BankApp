using BankApp.Interfaces;
using System;

namespace BankApp.Helpers.Factories
{
    public class PaymentCardNumberFactory : IPaymentCardNumberGenerator
    {
        private static readonly int visaPrefix = 4;

        public string GenerateCardNumber()
        {
            return Guid.NewGuid().ToString().Substring(0, 15);
        }
    }
}
