using System.Collections.Generic;

namespace BankApp.Models
{
    public class Manager : Worker
    {
        public IList<ManagerAtBranchHistory> ManagerAtBranchHistory { get; set; }
    }
}
