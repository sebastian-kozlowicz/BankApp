using System.Collections.Generic;

namespace BankApp.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string BranchCode { get; set; }
        public BranchAddress BranchAddress { get; set; }
        public Headquarters Headquarters { get; set; }
        public IList<Employee> Employees { get; set; }
        public IList<EmployeeAtBranchHistory> EmployeeAtBranchHistory { get; set; }
    }
}
