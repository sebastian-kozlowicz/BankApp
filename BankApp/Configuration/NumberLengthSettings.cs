namespace BankApp.Configuration
{
    public class NumberLengthSettings
    {
        public static class PaymentCard
        {
            public const int BankIdentificationNumber = 6;
            public const int BankIdentificationNumberAndCheckDigit = BankIdentificationNumber + 1;
        }

        public static class BankAccount
        {
            public const int Iban = 28;
        }
    }
}
