using System;
using System.Linq;
using BankApp.Configuration;
using BankApp.Data;
using BankApp.Enumerators;
using BankApp.Exceptions;
using BankApp.Interfaces.Builders;
using BankApp.Models;

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
            var bankAccount = _context.BankAccounts.SingleOrDefault(ba => ba.Id == bankAccountId);
            if (bankAccount == null)
                throw new ArgumentException($"Bank account with id {bankAccountId} doesn't exist.");

            if (!IssuingNetworkSettings.Mastercard.Length.AcceptedLengths.Contains(length))
                throw new ArgumentException("Requested Mastercard payment card number length is invalid.");

            var bankIdentificationNumber =
                _context.BankIdentificationNumberData.FirstOrDefault(bin =>
                    bin.IssuingNetwork == IssuingNetwork.Mastercard);
            if (bankIdentificationNumber == null)
                throw new InvalidDataInDatabaseException(
                    $"Bank identification number data for {IssuingNetwork.Mastercard} issuing network doesn't exist in database.");

            if (!IssuingNetworkSettings.Mastercard.Prefix.ValidPrefixes.Any(prefix =>
                bankIdentificationNumber.BankIdentificationNumber.ToString().StartsWith(prefix)))
                throw new InvalidDataInDatabaseException(
                    "Mastercard bank identification number found in database is invalid.");

            var accountIdentificationNumber =
                PaymentCardNumberBuilder.GetAccountIdentificationNumber(length, bankAccount.AccountNumberText);

            var maxAccountIdentificationNumber = _context.PaymentCards
                .Where(p => p.BankIdentificationNumber == bankIdentificationNumber.BankIdentificationNumber)
                .Where(p => p.AccountIdentificationNumber == long.Parse(accountIdentificationNumber))
                .Max(p => (int?)p.AccountIdentificationNumber);

            if (maxAccountIdentificationNumber != null)
                accountIdentificationNumber =
                    PaymentCardNumberBuilder.GetAccountIdentificationNumber(length,
                        ((int)maxAccountIdentificationNumber + 1).ToString("D16"));

            var paymentCardNumberWithoutCheckDigit =
                $"{bankIdentificationNumber.BankIdentificationNumber}{accountIdentificationNumber}";
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
                IssuingNetwork = IssuingNetwork.Mastercard
            };
        }
    }
}