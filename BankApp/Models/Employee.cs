using System.Collections.Generic;

namespace BankApp.Models
{
    public class Employee : Worker
    {
        public IList<EmployeeAtBranchHistory> EmployeeAtBranchHistory { get; set; }
    }
}
