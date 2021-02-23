using System.Linq;

namespace BankApp.Helpers.Builders
{
    public static class PaymentCardNumberBuilder
    {
        private static readonly int BankIdentificationNumberAndCheckDigitLength = 7;

        public static string GetAccountIdentificationNumber(int length, string accountNumberText)
        {
            return accountNumberText.Substring(0, length - BankIdentificationNumberAndCheckDigitLength);
        }

        public static string GenerateCheckDigit(string number)
        {
            var sum = 0;
            var oddPositon = false;
            var numberDigitsArray = number.Select(digit => int.Parse(digit.ToString())).ToArray().Reverse();

            foreach (var digit in numberDigitsArray)
            {
                if (oddPositon)
                {
                    var digitSquared = digit * 2;
                    var digitSquaredText = digitSquared.ToString();

                    if (digitSquaredText.Length >= 10)
                    {
                        var digitSquaredArray = digitSquaredText.Select(digit => int.Parse(digit.ToString())).ToArray();
                        var digitsSquaredSum = digitSquaredArray.Sum();
                        sum += digitsSquaredSum;
                    }
                }
                else
                {
                    sum += digit;
                }

                oddPositon = !oddPositon;
            }

            return ((10 - sum % 10) % 10).ToString();
        }
    }
}