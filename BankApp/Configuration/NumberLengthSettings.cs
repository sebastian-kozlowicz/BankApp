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
            public const int NationalBankCode = 4;
            public const int BranchCode = 3;
            public const int AccountNumber = 16;

        }
    }
}
