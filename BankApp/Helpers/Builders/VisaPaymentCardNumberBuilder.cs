using BankApp.Configuration;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Models;
using System;

namespace BankApp.Helpers.Builders
{
    public class VisaPaymentCardNumberBuilder : IPaymentCardNumberBuilder
    {
        private readonly BankIdentificationNumberData _bankIdentificationNumberData;
        private static readonly string VisaValidPrefix = "4";

        public VisaPaymentCardNumberBuilder(BankIdentificationNumberData bankIdentificationNumberData)
        {
            _bankIdentificationNumberData = bankIdentificationNumberData;
        }

        public PaymentCardNumber GeneratePaymentCardNumber(int length)
        {
            if (!VisaAcceptedLength.AcceptedLengths.Contains(length))
                throw new ArgumentException("Requested Visa payment card number length is invalid.");

            var bankIdentificationNumber = _bankIdentificationNumberData.GetBankIdentificationNumber(IssuingNetwork.Visa);
            if (!bankIdentificationNumber.BankIdentificationNumber.ToString().StartsWith(VisaValidPrefix))
                throw new ArgumentException("Visa bank identifiaction number found in database is invalid.");

            return null;
        }
    }
}
