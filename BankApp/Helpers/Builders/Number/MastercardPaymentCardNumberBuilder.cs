using System;
using System.Linq;
using BankApp.Configuration;
using BankApp.Data;
using BankApp.Enumerators;
using BankApp.Exceptions;
using BankApp.Interfaces.Helpers.Builders.Number;
using BankApp.Models;

namespace BankApp.Helpers.Builders.Number
{
    public class MastercardPaymentCardNumberBuilder : PaymentCardNumberBuilder, IPaymentCardNumberBuilder
    {
        public MastercardPaymentCardNumberBuilder(ApplicationDbContext context)
            : base(context)
        {
        }

        public PaymentCardNumber GeneratePaymentCardNumber(int length, int bankAccountId)
        {
            var bankAccount = Context.BankAccounts.SingleOrDefault(ba => ba.Id == bankAccountId);
            if (bankAccount == null)
                throw new ArgumentException($"Bank account with id {bankAccountId} doesn't exist.");

            if (!IssuingNetworkSettings.Mastercard.Length.AcceptedLengths.Contains(length))
                throw new ArgumentException("Requested Mastercard payment card number length is invalid.");

            var bankIdentificationNumber =
                Context.BankIdentificationNumberData.FirstOrDefault(bin =>
                    bin.IssuingNetwork == IssuingNetwork.Mastercard);
            if (bankIdentificationNumber == null)
                throw new InvalidDataInDatabaseException(
                    $"Bank identification number data for {IssuingNetwork.Mastercard} issuing network doesn't exist in database.");

            if (!IssuingNetworkSettings.Mastercard.Prefix.ValidPrefixes.Any(prefix =>
                bankIdentificationNumber.BankIdentificationNumber.ToString().StartsWith(prefix)))
                throw new InvalidDataInDatabaseException(
                    "Mastercard bank identification number found in database is invalid.");

            var accountIdentificationNumber = GenerateAccountIdentificationNumber();
            var accountIdentificationNumberText = GetAccountIdentificationNumberText(length, accountIdentificationNumber);
            var paymentCardNumberWithoutCheckDigit = $"{bankIdentificationNumber.BankIdentificationNumber}{accountIdentificationNumberText}";
            var checkDigit = GenerateCheckDigit(paymentCardNumberWithoutCheckDigit);
            var paymentCardNumber = $"{paymentCardNumberWithoutCheckDigit}{checkDigit}";

            return new PaymentCardNumber
            {
                MajorIndustryIdentifier = byte.Parse(paymentCardNumber.Substring(0, 1)),
                BankIdentificationNumber = bankIdentificationNumber.BankIdentificationNumber,
                AccountIdentificationNumber = accountIdentificationNumber,
                AccountIdentificationNumberText = accountIdentificationNumberText,
                CheckDigit = checkDigit,
                Number = paymentCardNumber,
                IssuingNetwork = IssuingNetwork.Mastercard
            };
        }
    }
}