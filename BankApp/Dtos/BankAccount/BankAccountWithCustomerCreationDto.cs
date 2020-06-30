using BankApp.Dtos.Auth;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Dtos.BankAccount
{
    public class BankAccountWithCustomerCreationDto
    {
        [Required]
        public RegisterDto RegisterDto { get; set; }
        [Required]
        public BankAccountCreationDto BankAccountCreationDto { get; set; }
    }
}
