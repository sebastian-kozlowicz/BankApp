using System.ComponentModel.DataAnnotations;
using BankApp.Dtos.Auth;

namespace BankApp.Dtos.BankAccount.WithCustomer
{
    public class BankAccountWithCustomerCreationByAnotherUserDto
    {
        [Required] 
        public RegisterByAnotherUserDto Register { get; set; }
        [Required] 
        public BankAccountCreationDto BankAccount { get; set; }
    }
}