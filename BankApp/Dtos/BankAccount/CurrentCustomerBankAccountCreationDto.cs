using BankApp.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Dtos.BankAccount
{
    public class CurrentCustomerBankAccountCreationDto
    {
        [Required]
        public AccountType? AccountType { get; set; }

        [Required]
        public Currency? Currency { get; set; }
    }
}
