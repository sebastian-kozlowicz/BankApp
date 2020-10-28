using System.ComponentModel.DataAnnotations;
using BankApp.Dtos.Address;
using BankApp.Dtos.ApplicationUser;

namespace BankApp.Dtos.Auth
{
    public class RegisterByAnotherUserDto
    {
        [Required]
        public ApplicationUserCreationByAnotherUserDto User { get; set; }
        [Required]
        public AddressCreationDto Address { get; set; }
    }
}
