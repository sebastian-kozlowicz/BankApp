using BankApp.Interfaces;
using BankApp.Models;

namespace BankApp.Helpers.Builders
{
    public class MastercardPaymentCardNumberBuilder : IPaymentCardNumberBuilder
    {
        //private static readonly int MastercardPrefix = 4;

        public PaymentCardNumber GeneratePaymentCardNumber(int length)
        {
            return null;
        }
    }
}
