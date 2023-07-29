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
    /// <summary>
    ///     Visa Payment card number builder
    ///     Payment card number structure:
    ///     - a six or eight-digit Issuer Identification Number (IIN) also called Bank Identification Number (BIN), the first
    ///     digit of which is the Major Industry Identifier (MII),
    ///     this number is assigned to bank for each issuing network (e.g. Visa, Mastercard)
    ///     - a variable length (up to 12 digits) individual account identifier
    ///     - a single check digit calculated using the Luhn algorithm
    ///     Each issuing network has its own payment card number length and starting digits
    ///     Mastercard issuing network rules:
    ///     - Length 16, starting digits within range 51–55, from 2017 also 2221–2720
    ///     Visa issuing network rules:
    ///     - Length 13, 16, 19, starting digit 4
    ///     Detailed explanation of payment card number structure, generation and validation is available on Wikipedia
    ///     https://en.wikipedia.org/wiki/Payment_card_number
    /// </summary>
    public class VisaPaymentCardNumberBuilder : PaymentCardNumberBuilder, IPaymentCardNumberBuilder
    {
        public VisaPaymentCardNumberBuilder(ApplicationDbContext context)
            : base(context)
        {
        }

        public PaymentCardNumber GeneratePaymentCardNumber(int length, int bankAccountId)
        {
            var bankAccount = Context.BankAccounts.SingleOrDefault(ba => ba.Id == bankAccountId);
            if (bankAccount == null)
                throw new ArgumentException($"Bank account with id {bankAccountId} doesn't exist.");

            if (!IssuingNetworkSettings.Visa.Length.AcceptedLengths.Contains(length))
                throw new ArgumentException("Requested Visa payment card number length is invalid.");

            var bankIdentificationNumber =
                Context.BankIdentificationNumberData.FirstOrDefault(bin => bin.IssuingNetwork == IssuingNetwork.Visa);
            if (bankIdentificationNumber == null)
                throw new InvalidDataInDatabaseException(
                    $"Bank identification number data for {IssuingNetwork.Visa} issuing network doesn't exist in database.");

            if (!IssuingNetworkSettings.Visa.Prefix.ValidPrefixes.Any(prefix =>
                bankIdentificationNumber.BankIdentificationNumber.ToString().StartsWith(prefix)))
                throw new InvalidDataInDatabaseException(
                    "Visa bank identification number found in database is invalid.");

            var accountIdentificationNumber = GenerateAccountIdentificationNumber();
            var accountIdentificationNumberText =
                GetAccountIdentificationNumberText(length, accountIdentificationNumber);
            var paymentCardNumberWithoutCheckDigit =
                $"{bankIdentificationNumber.BankIdentificationNumber}{accountIdentificationNumberText}";
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
                IssuingNetwork = IssuingNetwork.Visa
            };
        }
    }
}