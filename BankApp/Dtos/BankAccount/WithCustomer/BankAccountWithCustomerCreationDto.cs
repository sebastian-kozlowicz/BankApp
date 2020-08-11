using BankApp.Dtos.Auth;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Dtos.BankAccount.WithCustomer
{
    public class BankAccountWithCustomerCreationDto
    {
        [Required]
        public RegisterDto Register { get; set; }
        [Required]
        public BankAccountCreationDto BankAccount { get; set; }
    }
}
