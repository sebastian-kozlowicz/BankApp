using BankApp.Enumerators;

namespace BankApp.Models
{
    public class BankAccount
    {
        public int Id { get; set; }
        public AccountType AccountType { get; set; }
        public Currency Currency { get; set; }
        public string CountryCode { get; set; } = "PL";
        public int CheckNumber { get; set; }
        public int NationalBankCode { get; set; }
        public int BranchCode { get; set; }
        public int NationalCheckDigit { get; set; }
        public long AccountNumber { get; set; }
        public string Iban { get; set; }
        public decimal Balance { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public string ApplicationUserId { get; set; }
    }
}
