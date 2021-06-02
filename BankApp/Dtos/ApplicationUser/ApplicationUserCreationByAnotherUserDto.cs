using System.ComponentModel.DataAnnotations;
using BankApp.Attributes;

namespace BankApp.Dtos.ApplicationUser
{
    public class ApplicationUserCreationByAnotherUserDto
    {
        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Name { get; set; }

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 2)]
        public string Surname { get; set; }

        [Required]
        [EmailAddress]
        [SensitiveData]
        public string Email { get; set; }

        [Required]
        [StringLength(12, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
    }
}