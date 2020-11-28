using System;

namespace BankApp.Models
{
    public class TellerAtBranchHistory
    {
        public int Id { get; set; }
        public DateTime AssignDate { get; set; }
        public DateTime? ExpelDate { get; set; }
        public Branch Branch { get; set; }
        public int BranchId { get; set; }
        public Teller Teller { get; set; }
        public int TellerId { get; set; }
        public ApplicationUser AssignedBy { get; set; }
        public int AssignedById { get; set; }
        public ApplicationUser ExpelledBy { get; set; }
        public int? ExpelledById { get; set; }
    }
}
