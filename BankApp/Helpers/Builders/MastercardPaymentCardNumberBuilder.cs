using BankApp.Configuration;
using BankApp.Data;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Models;
using System;
using System.Linq;

namespace BankApp.Helpers.Builders
{
    public class MastercardPaymentCardNumberBuilder : IPaymentCardNumberBuilder
    {
        private readonly ApplicationDbContext _context;

        public MastercardPaymentCardNumberBuilder(ApplicationDbContext context)
        {
            _context = context;
        }

        public PaymentCardNumber GeneratePaymentCardNumber(int length, int bankAccountId)
        {
            if (!IssuingNetworkSettings.Mastercard.Length.AcceptedLengths.Contains(length))
                throw new ArgumentException("Requested Mastercard payment card number length is invalid.");

            var bankIdentificationNumber = _context.BankIdentificationNumberData.FirstOrDefault(bin => bin.IssuingNetwork == IssuingNetwork.Mastercard);
            if (!IssuingNetworkSettings.Mastercard.Prefix.ValidPrefixes.Any(prefix => bankIdentificationNumber.BankIdentificationNumber.ToString().StartsWith(prefix)))
                throw new ArgumentException("Mastercard bank identifiaction number found in database is invalid.");

            var bankAccount = _context.BankAccounts.SingleOrDefault(ba => ba.Id == bankAccountId);
            var accountIdentificationNumber = PaymentCardNumberBuilder.GetAccountIdentificationNumber(length, bankAccount.AccountNumberText);
            var numberWithoutCheckDigit = $"{bankIdentificationNumber.BankIdentificationNumber}{accountIdentificationNumber}";
            var checkDigit = PaymentCardNumberBuilder.GenerateCheckDigit(numberWithoutCheckDigit);

            return null;
        }
    }
}
