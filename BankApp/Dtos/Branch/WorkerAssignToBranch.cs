using System.ComponentModel.DataAnnotations;

namespace BankApp.Dtos.Branch
{
    public class WorkerAssignToBranch
    {
        [Required]
        public int WorkerId { get; set; }
        [Required]
        public int BranchId { get; set; }
    }
}
