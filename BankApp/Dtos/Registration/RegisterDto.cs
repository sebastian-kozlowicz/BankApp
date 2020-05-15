using System.ComponentModel.DataAnnotations;

namespace BankApp.Dtos.Registration
{
    public class RegisterDto
    {
        [Required]
        public UserCreationDto User { get; set; }
        [Required]
        public AddressDto Address { get; set; }
    }
}
