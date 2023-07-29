﻿using System.Linq;
using BankApp.Data;

namespace BankApp.Helpers.Builders.Number
{
    /// <summary>
    ///     Payment card number builder base class
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
    public abstract class PaymentCardNumberBuilder
    {
        private const int BankIdentificationNumberAndCheckDigitLength = 7;
        protected readonly ApplicationDbContext Context;

        protected PaymentCardNumberBuilder(ApplicationDbContext context)
        {
            Context = context;
        }

        protected long GenerateAccountIdentificationNumber()
        {
            var maxAccountIdentificationNumber = Context.PaymentCards
                .Max(p => (long?)p.AccountIdentificationNumber);

            if (maxAccountIdentificationNumber == null)
                return 0;

            return (long)(maxAccountIdentificationNumber + 1);
        }

        protected string GetAccountIdentificationNumberText(int length, long accountNumber) =>
            accountNumber.ToString($"D{length - BankIdentificationNumberAndCheckDigitLength}");

        /// <summary>
        ///     Generates payment card's check digit using Luhn algorithm https://en.wikipedia.org/wiki/Luhn_algorithm
        /// </summary>
        /// <param name="number">payment card number without check digit</param>
        /// <returns></returns>
        public byte GenerateCheckDigit(string number)
        {
            var sum = 0;
            var oddPosition = false;
            number += "0";
            var numberDigitsArray = number.Select(digit => int.Parse(digit.ToString())).ToArray().Reverse();

            foreach (var digit in numberDigitsArray)
            {
                if (oddPosition)
                {
                    var digitSquared = digit * 2;
                    var digitSquaredText = digitSquared.ToString();

                    if (digitSquaredText.Length >= 2)
                    {
                        var digitSquaredArray = digitSquaredText.Select(digit => int.Parse(digit.ToString())).ToArray();
                        var digitsSquaredSum = digitSquaredArray.Sum();
                        sum += digitsSquaredSum;
                    }
                    else
                    {
                        sum += digitSquared;
                    }
                }
                else
                {
                    sum += digit;
                }

                oddPosition = !oddPosition;
            }

            return (byte)(10 - sum % 10);
        }
    }
}