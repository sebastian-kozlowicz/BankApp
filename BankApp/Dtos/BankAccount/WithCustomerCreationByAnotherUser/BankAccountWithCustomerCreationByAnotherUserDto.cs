using System.ComponentModel.DataAnnotations;
using BankApp.Dtos.Auth;

namespace BankApp.Dtos.BankAccount.WithCustomerCreationByAnotherUser
{
    public class BankAccountWithCustomerCreationByAnotherUserDto
    {
        [Required] 
        public RegisterByAnotherUserDto Register { get; set; }
        [Required] 
        public BankAccountCreationDto BankAccount { get; set; }
    }
}