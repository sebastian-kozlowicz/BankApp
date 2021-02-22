using BankApp.Configuration;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Models;
using System;
using System.Linq;

namespace BankApp.Helpers.Builders
{
    public class MastercardPaymentCardNumberBuilder : IPaymentCardNumberBuilder
    {
        private readonly BankIdentificationNumberData _bankIdentificationNumberData;
        private readonly BankAccount _bankAccount;

        public MastercardPaymentCardNumberBuilder(BankIdentificationNumberData bankIdentificationNumberData, BankAccount bankAccount)
        {
            _bankIdentificationNumberData = bankIdentificationNumberData;
            _bankAccount = bankAccount;
        }

        public PaymentCardNumber GeneratePaymentCardNumber(int length, int bankAccountId)
        {
            if (!IssuingNetworkSettings.Mastercard.Length.AcceptedLengths.Contains(length))
                throw new ArgumentException("Requested Mastercard payment card number length is invalid.");

            var bankIdentificationNumber = _bankIdentificationNumberData.GetBankIdentificationNumber(IssuingNetwork.Mastercard);
            if (!IssuingNetworkSettings.Mastercard.Prefix.ValidPrefixes.Any(prefix => bankIdentificationNumber.BankIdentificationNumber.ToString().StartsWith(prefix)))
                throw new ArgumentException("Mastercard bank identifiaction number found in database is invalid.");

            var bankAccount = _bankAccount.GetBankAccount(bankAccountId);

            return null;
        }
    }
}
