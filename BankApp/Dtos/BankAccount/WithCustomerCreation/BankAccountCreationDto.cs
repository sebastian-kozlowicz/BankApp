using System.ComponentModel.DataAnnotations;
using BankApp.Enumerators;

namespace BankApp.Dtos.BankAccount.WithCustomerCreation
{
    public class BankAccountCreationDto
    {
        [Required]
        public AccountType? AccountType { get; set; }
        [Required]
        public Currency? Currency { get; set; }
    }
}
