using BankApp.Interfaces;
using BankApp.Models;

namespace BankApp.Helpers.Builders
{
    public class VisaPaymentCardNumberBuilder : IPaymentCardNumberBuilder
    {
        private static readonly int VisaPrefix = 4;

        public PaymentCardNumber GeneratePaymentCardNumber(int length)
        {
            return null;
        }
    }
}
