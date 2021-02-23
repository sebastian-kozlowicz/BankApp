using BankApp.Configuration;
using BankApp.Data;
using BankApp.Enumerators;
using BankApp.Interfaces;
using BankApp.Models;
using System;
using System.Linq;

namespace BankApp.Helpers.Builders
{
    public class VisaPaymentCardNumberBuilder : IPaymentCardNumberBuilder
    {
        private readonly ApplicationDbContext _context;

        public VisaPaymentCardNumberBuilder(ApplicationDbContext context)
        {
            _context = context;
        }

        public PaymentCardNumber GeneratePaymentCardNumber(int length, int bankAccountId)
        {
            if (!IssuingNetworkSettings.Visa.Length.AcceptedLengths.Contains(length))
                throw new ArgumentException("Requested Visa payment card number length is invalid.");

            var bankIdentificationNumber = _context.BankIdentificationNumberData.FirstOrDefault(bin => bin.IssuingNetwork == IssuingNetwork.Visa);
            if (!IssuingNetworkSettings.Visa.Prefix.ValidPrefixes.Any(prefix => bankIdentificationNumber.BankIdentificationNumber.ToString().StartsWith(prefix)))
                throw new ArgumentException("Visa bank identifiaction number found in database is invalid.");

            var bankAccount = _context.BankAccounts.SingleOrDefault(ba => ba.Id == bankAccountId);
            var accountIdentificationNumber = PaymentCardNumberBuilder.GetAccountIdentificationNumber(length, bankAccount.AccountNumberText);

            return null;
        }
    }
}
