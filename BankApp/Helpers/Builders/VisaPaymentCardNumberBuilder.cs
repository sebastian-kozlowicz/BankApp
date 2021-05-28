using BankApp.Configuration;
using BankApp.Data;
using BankApp.Enumerators;
using BankApp.Models;
using System;
using System.Linq;
using BankApp.Exceptions;
using BankApp.Interfaces.Builders;

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
            var bankAccount = _context.BankAccounts.SingleOrDefault(ba => ba.Id == bankAccountId);
            if (bankAccount == null)
                throw new ArgumentException($"Bank account with id {bankAccountId} doesn't exist.");

            if (!IssuingNetworkSettings.Visa.Length.AcceptedLengths.Contains(length))
                throw new ArgumentException("Requested Visa payment card number length is invalid.");

            var bankIdentificationNumber = _context.BankIdentificationNumberData.FirstOrDefault(bin => bin.IssuingNetwork == IssuingNetwork.Visa);
            if (bankIdentificationNumber == null)
                throw new InvalidDataInDatabaseException($"Bank identification number data for {IssuingNetwork.Visa} issuing network doesn't exist in database.");

            if (!IssuingNetworkSettings.Visa.Prefix.ValidPrefixes.Any(prefix => bankIdentificationNumber.BankIdentificationNumber.ToString().StartsWith(prefix)))
                throw new InvalidDataInDatabaseException("Visa bank identification number found in database is invalid.");

            var accountIdentificationNumber = PaymentCardNumberBuilder.GetAccountIdentificationNumber(length, bankAccount.AccountNumberText);
            var paymentCardNumberWithoutCheckDigit = $"{bankIdentificationNumber.BankIdentificationNumber}{accountIdentificationNumber}";
            var checkDigit = PaymentCardNumberBuilder.GenerateCheckDigit(paymentCardNumberWithoutCheckDigit);
            var paymentCardNumber = $"{paymentCardNumberWithoutCheckDigit}{checkDigit}";

            return new PaymentCardNumber
            {
                MajorIndustryIdentifier = byte.Parse(paymentCardNumber.Substring(0, 1)),
                BankIdentificationNumber = bankIdentificationNumber.BankIdentificationNumber,
                AccountIdentificationNumber = long.Parse(accountIdentificationNumber),
                AccountIdentificationNumberText = accountIdentificationNumber,
                CheckDigit = checkDigit,
                Number = paymentCardNumber,
                IssuingNetwork = IssuingNetwork.Visa
            };
        }
    }
}
