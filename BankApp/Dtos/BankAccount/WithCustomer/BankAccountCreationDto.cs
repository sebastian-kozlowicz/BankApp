using BankApp.Enumerators;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Dtos.BankAccount.WithCustomer
{
    public class BankAccountCreationDto
    {
        [Required]
        public AccountType AccountType { get; set; }
        [Required]
        public Currency Currency { get; set; }
    }
}
