using BankApp.Configuration;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Models;
using System;
using System.Linq;

namespace BankApp.Helpers.Builders
{
    public class VisaPaymentCardNumberBuilder : IPaymentCardNumberBuilder
    {
        private readonly BankIdentificationNumberData _bankIdentificationNumberData;

        public VisaPaymentCardNumberBuilder(BankIdentificationNumberData bankIdentificationNumberData)
        {
            _bankIdentificationNumberData = bankIdentificationNumberData;
        }

        public PaymentCardNumber GeneratePaymentCardNumber(int length, int bankAccountId)
        {
            if (!IssuingNetworkSettings.Visa.Length.AcceptedLengths.Contains(length))
                throw new ArgumentException("Requested Visa payment card number length is invalid.");

            var bankIdentificationNumber = _bankIdentificationNumberData.GetBankIdentificationNumber(IssuingNetwork.Visa);
            if (!IssuingNetworkSettings.Visa.Prefix.ValidPrefixes.Any(prefix => bankIdentificationNumber.BankIdentificationNumber.ToString().StartsWith(prefix)))
                throw new ArgumentException("Visa bank identifiaction number found in database is invalid.");

            return null;
        }
    }
}
