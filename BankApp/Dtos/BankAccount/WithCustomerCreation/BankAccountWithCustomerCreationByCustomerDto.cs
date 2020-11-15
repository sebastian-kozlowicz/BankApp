using System.ComponentModel.DataAnnotations;
using BankApp.Dtos.Auth;

namespace BankApp.Dtos.BankAccount.WithCustomerCreation
{
    public class BankAccountWithCustomerCreationByCustomerDto
    {
        [Required]
        public RegisterDto Register { get; set; }
        [Required]
        public BankAccountCreationDto BankAccount { get; set; }
    }
}
