using BankApp.Interfaces;
using System;

namespace BankApp.Helpers.Factories
{
    public class PaymentCardNumberFactory : IPaymentCardNumberFactory
    {
        public string GenerateCardNumber()
        {
            return Guid.NewGuid().ToString().Substring(0, 15);
        }
    }
}
