using System.Collections.Generic;

namespace BankApp.Models
{
    public class Branch
    {
        public int Id { get; set; }
        public string BranchCode { get; set; }
        public BranchAddress BranchAddress { get; set; }
        public Headquarters Headquarters { get; set; }
        public IList<Teller> AssignedTellers { get; set; }
        public IList<Manager> AssignedManagers { get; set; }
        public IList<TellerAtBranchHistory> TellerAtBranchHistory { get; set; }
        public IList<ManagerAtBranchHistory> ManagerAtBranchHistory { get; set; }
    }
}
