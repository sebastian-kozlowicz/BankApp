using BankApp.Dtos.ApplicationUser;
using BankApp.Dtos.Customer;
using BankApp.Enumerators;

namespace BankApp.Dtos.BankAccount
{
    public class BankAccountDto
    {
        public int Id { get; set; }
        public AccountType AccountType { get; set; }
        public Currency Currency { get; set; }
        public string CountryCode { get; set; }
        public string CheckNumber { get; set; }
        public string NationalBankCode { get; set; }
        public string BranchCode { get; set; }
        public int NationalCheckDigit { get; set; }
        public long AccountNumber { get; set; }
        public string AccountNumberText { get; set; }
        public string Iban { get; set; }
        public string IbanSeparated { get; set; }
        public decimal Balance { get; set; }
        public decimal DebitLimit { get; set; }
        public CustomerDto Customer { get; set; }
        public int CustomerId { get; set; }
        public ApplicationUserDto CreatedBy { get; set; }
        public int CreatedById { get; set; }
    }
}
