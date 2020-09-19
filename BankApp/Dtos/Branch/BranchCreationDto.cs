using System.ComponentModel.DataAnnotations;

namespace BankApp.Dtos.Branch
{
    public class BranchCreationDto
    {
        [Required]
        public string BranchCode { get; set; }
    }
}
