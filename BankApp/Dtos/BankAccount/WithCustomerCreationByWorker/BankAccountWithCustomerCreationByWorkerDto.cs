using System.ComponentModel.DataAnnotations;
using BankApp.Dtos.Auth;

namespace BankApp.Dtos.BankAccount.WithCustomerCreationByWorker
{
    public class BankAccountWithCustomerCreationByWorkerDto
    {
        [Required] 
        public RegisterByAnotherUserDto Register { get; set; }
        [Required] 
        public BankAccountCreationDto BankAccount { get; set; }
    }
}