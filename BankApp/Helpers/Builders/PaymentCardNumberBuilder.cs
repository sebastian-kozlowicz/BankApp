using System.Linq;

namespace BankApp.Helpers.Builders
{
    public static class PaymentCardNumberBuilder
    {
        private const int BankIdentificationNumberAndCheckDigitLength = 7;

        public static string GetAccountIdentificationNumber(int length, string accountNumberText)
        {
            return accountNumberText.Substring(length - BankIdentificationNumberAndCheckDigitLength, length);
        }

        public static byte GenerateCheckDigit(string number)
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