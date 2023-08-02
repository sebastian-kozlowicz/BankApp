using BankApp.Models;

namespace BankApp.Interfaces.Helpers.Builders.Number
{
    public interface IBankAccountNumberBuilder
    {
        BankAccountNumber GenerateBankAccountNumber(int? branchId = null);
        int GenerateNationalCheckDigit(string nationalBankCode, string branchCode);

        string GenerateCheckDigits(BankData bankData, string branchCode, int nationalCheckDigit,
            string accountNumberText);

        bool ValidateBankAccountNumber(string iban);

        string GetIban(BankData bankData, string checkDigits, string branchCode, int nationalCheckDigit,
            string accountNumberText);

        string GetIbanSeparated(BankData bankData, string checkNumber, string branchCode, int nationalCheckDigit,
            string accountNumberText);
    }
}