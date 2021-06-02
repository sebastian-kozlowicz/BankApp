using System.Linq;
using BankApp.Data;

namespace BankApp.Helpers.Builders.Number
{
    public abstract class PaymentCardNumberBuilder
    {
        protected readonly ApplicationDbContext Context;
        private const int BankIdentificationNumberAndCheckDigitLength = 7;

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

        protected string GetAccountIdentificationNumber(int length, long accountNumber)
        {
            return accountNumber.ToString($"D{length - BankIdentificationNumberAndCheckDigitLength}");
        }

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

            return (byte) ((10 - sum % 10) % 10);
        }
    }
}