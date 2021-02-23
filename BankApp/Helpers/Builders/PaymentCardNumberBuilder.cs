namespace BankApp.Helpers.Builders
{
    public static class PaymentCardNumberBuilder
    {
        private static readonly int BankIdentificationNumberAndCheckDigitLength = 7;

        public static string GetAccountIdentificationNumber(int length, string accountNumberText)
        {
            return accountNumberText.Substring(0, length - BankIdentificationNumberAndCheckDigitLength);
        }
    }
}
