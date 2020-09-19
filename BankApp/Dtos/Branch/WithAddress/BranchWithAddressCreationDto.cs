using BankApp.Dtos.Address;
using System.ComponentModel.DataAnnotations;

namespace BankApp.Dtos.Branch.WithAddress
{
    public class BranchWithAddressCreationDto
    {
        [Required]
        public BranchCreationDto Branch { get; set; }
        [Required]
        public AddressCreationDto Address { get; set; }
    }
}
