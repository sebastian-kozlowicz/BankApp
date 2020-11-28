using System.Collections.Generic;

namespace BankApp.Models
{
    public class Teller : Worker
    {
        public IList<TellerAtBranchHistory> TellerAtBranchHistory { get; set; }
    }
}
