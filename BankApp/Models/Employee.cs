using System.Collections.Generic;

namespace BankApp.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public ApplicationUser ApplicationUser { get; set; }
        public Branch WorkAt { get; set; }
        public int? WorkAtId { get; set; }
        public IList<EmployeeAtBranchHistory> EmployeeAtBranchHistory { get; set; }
    }
}
