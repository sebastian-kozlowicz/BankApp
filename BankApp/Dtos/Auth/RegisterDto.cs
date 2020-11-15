using BankApp.Dtos.Address;
using BankApp.Dtos.ApplicationUser;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Dtos.Auth
{
    public class RegisterDto
    {
        [Required]
        public ApplicationUserCreationBySameUserDto User { get; set; }
        [Required]
        public AddressCreationDto Address { get; set; }
    }
}
