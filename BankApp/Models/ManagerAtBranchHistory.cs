using System;

namespace BankApp.Models
{
    public class ManagerAtBranchHistory
    {
        public int Id { get; set; }
        public DateTime AssignDate { get; set; }
        public DateTime? ExpelDate { get; set; }
        public Branch Branch { get; set; }
        public int BranchId { get; set; }
        public Manager Manager { get; set; }
        public int ManagerId { get; set; }
    }
}