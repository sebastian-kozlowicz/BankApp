using BankApp.Configuration;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BankApp.Helpers.Builders
{
    public class MastercardPaymentCardNumberBuilder : IPaymentCardNumberBuilder
    {
        private readonly BankIdentificationNumberData _bankIdentificationNumberData;
        private static readonly List<string> MastercardValidPrefixes = new List<string> { "51", "52", "53", "54", "55" };

        public MastercardPaymentCardNumberBuilder(BankIdentificationNumberData bankIdentificationNumberData)
        {
            _bankIdentificationNumberData = bankIdentificationNumberData;
        }

        public PaymentCardNumber GeneratePaymentCardNumber(int length)
        {
            if (!IssuingNetworkSettings.Mastercard.Length.AcceptedLengths.Contains(length))
                throw new ArgumentException("Requested Mastercard payment card number length is invalid.");

            var bankIdentificationNumber = _bankIdentificationNumberData.GetBankIdentificationNumber(IssuingNetwork.Mastercard);
            if (!MastercardValidPrefixes.Any(prefix => bankIdentificationNumber.BankIdentificationNumber.ToString().StartsWith(prefix)))
                throw new ArgumentException("Mastercard bank identifiaction number found in database is invalid.");
            return null;
        }
    }
}
