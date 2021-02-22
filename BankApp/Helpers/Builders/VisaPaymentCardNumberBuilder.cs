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
        private readonly BankAccount _bankAccount;

        public VisaPaymentCardNumberBuilder(BankIdentificationNumberData bankIdentificationNumberData, BankAccount bankAccount)
        {
            _bankIdentificationNumberData = bankIdentificationNumberData;
            _bankAccount = bankAccount;
        }

        public PaymentCardNumber GeneratePaymentCardNumber(int length, int bankAccountId)
        {
            if (!IssuingNetworkSettings.Visa.Length.AcceptedLengths.Contains(length))
                throw new ArgumentException("Requested Visa payment card number length is invalid.");

            var bankIdentificationNumber = _bankIdentificationNumberData.GetBankIdentificationNumber(IssuingNetwork.Visa);
            if (!IssuingNetworkSettings.Visa.Prefix.ValidPrefixes.Any(prefix => bankIdentificationNumber.BankIdentificationNumber.ToString().StartsWith(prefix)))
                throw new ArgumentException("Visa bank identifiaction number found in database is invalid.");

            var bankAccount = _bankAccount.GetBankAccount(bankAccountId);

            return null;
        }
    }
}
