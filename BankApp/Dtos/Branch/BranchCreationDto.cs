using System.ComponentModel.DataAnnotations;

namespace BankApp.Dtos.Branch
{
    public class BranchCreationDto
    {
        [Required]
        [RegularExpression(@"^\d{3}$", ErrorMessage = "String length can only contain digits and must be of length 3 characters.")]
        public string BranchCode { get; set; }
    }
}
