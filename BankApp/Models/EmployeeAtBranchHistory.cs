using System;

namespace BankApp.Models
{
    public class EmployeeAtBranchHistory
    {
        public int Id { get; set; }
        public DateTime AssignDate { get; set; }
        public DateTime? ExpelDate { get; set; }
        public Branch Branch { get; set; }
        public int BranchId { get; set; }
        public Employee Employee { get; set; }
        public int EmployeeId { get; set; }
    }
}
