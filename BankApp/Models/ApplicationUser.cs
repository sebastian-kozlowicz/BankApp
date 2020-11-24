using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BankApp.Models
{
    public class ApplicationUser : IdentityUser<int>
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Administrator Administrator { get; set; }
        public Customer Customer { get; set; }
        public Employee Employee { get; set; }
        public Manager Manager { get; set; }
        public UserAddress UserAddress { get; set; }
        public ApplicationUser CreatedBy { get; set; }
        public int? CreatedById { get; set; }
        public IList<EmployeeAtBranchHistory> AssignedEmployeesToBranchHistory { get; set; }
        public IList<EmployeeAtBranchHistory> ExpelledEmployeesFromBranchHistory { get; set; }
        public IList<ManagerAtBranchHistory> AssignedManagersToBranchHistory { get; set; }
        public IList<ManagerAtBranchHistory> ExpelledManagersFromBranchHistory { get; set; }
    }
}
