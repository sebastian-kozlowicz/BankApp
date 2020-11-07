using System.ComponentModel.DataAnnotations;

namespace BankApp.Dtos.Branch
{
    public class WorkerAtBranchDto
    {
        [Required]
        public int? WorkerId { get; set; }
        [Required]
        public int? BranchId { get; set; }
    }
}
