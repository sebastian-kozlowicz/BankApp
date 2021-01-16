using BankApp.Interfaces;
using System;

namespace BankApp.Helpers.Builders
{
    public class MastercardPaymentCardNumberBuilder : IPaymentCardNumberBuilder
    {
        //private static readonly int MastercardPrefix = 4;

        public string GenerateCardNumber(int length)
        {
            return Guid.NewGuid().ToString().Substring(0, 15);
        }
    }
}
