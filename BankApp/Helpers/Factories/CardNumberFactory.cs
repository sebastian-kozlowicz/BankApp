using BankApp.Interfaces;
using System;

namespace BankApp.Helpers.Factories
{
    public class CardNumberFactory : ICardNumberFactory
    {
        public string GenerateCardNumber()
        {
            return Guid.NewGuid().ToString().Substring(0, 15);
        }
    }
}
